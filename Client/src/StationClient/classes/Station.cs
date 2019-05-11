using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Net.Sockets;

namespace StationClient
{
    public class Station
    {
        private List<Track> tracks;
        private FakeApi fakeApi;
        private Connection connection;
        private ConnectionState connectionState;
        public string StationName { get; private set; }

        public Station(string stationName, List<Track> tracks, string apiFileName, string serverIp, int serverPort)
        {
            StationName = stationName;
            this.tracks = tracks;
            fakeApi = new FakeApi(apiFileName);

            IPAddress ip = IPAddress.Parse(serverIp);
            IPEndPoint server = new IPEndPoint(ip, serverPort);

            connection = new Connection(ip, server);
            connectionState = ConnectionState.Connecting;
        }

        public void LoadApi()
        {
            fakeApi.LoadFile();
        }

        public void ConnectToServer()
        {
            connection.Connect();
            connectionState = ConnectionState.Identify;
        }

        public void ReceiveMessages()
        {
            if (connectionState != ConnectionState.Connecting)
            {
                List<Message> messages = connection.ReceiveMessage();
                switch (connectionState)
                {
                    case ConnectionState.Identify:
                        connection.SendMessage("CONNECT:STATION");
                        connectionState = ConnectionState.Identifying;
                        break;
                    case ConnectionState.Identifying:
                        foreach (Message msg in messages)
                        {
                            if (msg.Command == "ACK")
                            {
                                connectionState = ConnectionState.Register;
                            }
                            else if (msg.Command == "NACK")
                            {
                                connectionState = ConnectionState.Identify;
                            }
                        }
                        break;
                    case ConnectionState.Register:
                        connection.SendMessage("IAM:" + StationName);
                        connectionState = ConnectionState.Registering;
                        break;
                    case ConnectionState.Registering:
                        foreach (Message msg in messages)
                        {
                            if (msg.Command == "ACK")
                            {
                                connectionState = ConnectionState.Registerd;
                            }
                            else if (msg.Command == "NACK")
                            {
                                connectionState = ConnectionState.Register;
                            }
                        }
                        break;
                    case ConnectionState.Registerd:
                        foreach (Message msg in messages)
                        {
                            if (msg.Command == "OCCUPATION")
                            {
                                List<string> information = msg.Values.Split("@").ToList();
                                List<List<int>> occuaptions = new List<List<int>>();
                                int rideNumber = 0;

                                if (Int32.TryParse(information[0], out rideNumber))
                                {
                                    information.RemoveAt(0);

                                    foreach (Track track in tracks)
                                    {
                                        if (track.RideNumber == rideNumber)
                                        {
                                            foreach (string info in information)
                                            {
                                                occuaptions.Add(info.Split(',').Select(int.Parse).ToList());
                                            }

                                            List<int> occupation = new List<int>();
                                            foreach (int trainUnit in fakeApi.GetTrainUnits(rideNumber))
                                            {
                                                foreach (List<int> occu in occuaptions)
                                                {
                                                    if (occu[0] == trainUnit)
                                                    {
                                                        for (int i = 1; i < occu.Count; i++)
                                                        {
                                                            occupation.Add(occu[i]);
                                                        }
                                                    }
                                                }
                                            }

                                            track.UpdateLamps(occupation);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }

        public void UpdateTracks()
        {
            //Loop trough tracks
            //Ask FakeApi what the next RideId to arrive is
            //Make message for server
            //Send server request for occupation of RideId
            foreach (Track track in tracks)
            {
                int rideNumber = fakeApi.GetRideByTrackId(track.TrackId);
                if (rideNumber > 0)
                {
                    track.RideNumber = rideNumber;
                    connection.SendMessage("GETOCCUPATION:" + rideNumber);
                }
            }
        }
    }
}