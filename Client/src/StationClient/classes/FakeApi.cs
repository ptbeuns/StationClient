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
    class FakeApi
    {
        private string fileName;
        private JObject jsonFile;
        public FakeApi(string fileName)
        {
            this.fileName = fileName;
            jsonFile = null;
        }

        public void LoadFile()
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

                jsonFile = JObject.Parse(Encoding.ASCII.GetString(bytes));
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }

        public int GetRideByTrackId(int trackId)
        {
            var temp =
            from p in jsonFile["Rides"]
            where (int)p["TrackId"] == trackId
            select (int)p["RideNumber"];

            if (temp.Count() > 0)
            {
                return temp.ToArray()[0];
            }
            else
            {
                return -1;
            }
        }

        public List<int> GetTrainUnits(int rideNumber)
        {
            List<int> trainUnits = new List<int>();
            var temp =
            from p in jsonFile["Rides"]
            where (int)p["RideNumber"] == rideNumber
            select (JArray)p["TrainUnits"];

            foreach(JArray units in temp)
            {
                trainUnits = units.ToObject<List<int>>();
            }

            return trainUnits;
        }
    }
}