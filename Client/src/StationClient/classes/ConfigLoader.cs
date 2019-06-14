using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StationClient
{
    class ConfigLoader
    {
        private string fileName;
        private JObject config;
        public ConfigLoader(string fileName)
        {
            this.fileName = fileName;
        }

        public void OpenConfig()
        {
            Stream stream = null;

            try
            {
                stream = File.Open(fileName, FileMode.Open);
                byte[] bytes = new byte[stream.Length + 10];
                long numBytesToRead = stream.Length;
                int numBytesRead = 0;
                do
                {
                    int n = stream.Read(bytes, numBytesRead, 10);
                    numBytesRead += n;
                    numBytesToRead -= n;
                }
                while (numBytesToRead > 0);

                config = JObject.Parse(Encoding.ASCII.GetString(bytes));
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }

        public Station LoadStationFromConfig()
        {
            string stationName = config.SelectToken("StationName").ToString();
            string apiFileName = config.SelectToken("ApiFile").ToString();
            string serverIp = config.SelectToken("ServerIp").ToString();
            int port = config.SelectToken("ServerPort").ToObject<int>();
            JToken[] jTracks = config.GetValue("Tracks").ToArray();
            List<Track> tracks = new List<Track>();

            foreach (JToken jTrack in jTracks)
            {
                int trackId = jTrack.SelectToken("TrackId").ToObject<int>();
                JToken[] jLamps = jTrack["I2CLamps"].ToArray();
                List<LedLamp> lamps = new List<LedLamp>();
                foreach(JToken jLamp in jLamps)
                {
                    lamps.Add(new LedLamp(jLamp.Value<int>()));
                }
                tracks.Add(new Track(trackId, lamps));
            }

            return new Station(stationName, tracks, apiFileName, serverIp, port);
        }
    }
}