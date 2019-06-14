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
                    case ConnectionState.Identifying:
                        foreach (Message msg in messages)
                        {
                            if (msg.Command == "ACK")
                            {
                                connectionState = ConnectionState.Register;
                                Console.WriteLine("Identified");
                            }
                            else if (msg.Command == "NACK")
                            {
                                connectionState = ConnectionState.Identify;
                                Console.WriteLine("Going back to identify");
                            }
                        }
                        break;
                    case ConnectionState.Registering:
                        foreach (Message msg in messages)
                        {
                            if (msg.Command == "ACK")
                            {
                                connectionState = ConnectionState.Connected;
                                Console.WriteLine("Connected");
                            }
                            else if (msg.Command == "NACK")
                            {
                                connectionState = ConnectionState.Register;
                                Console.WriteLine("Going back to register");
                            }
                        }
                        break;
                    case ConnectionState.Connected:
                        foreach (Message msg in messages)
                        {
                            if (msg.Command == "OCCUPATION")
                            {
                                Console.WriteLine("Receiving occupation");
                                Console.WriteLine(msg.Command);
                                Console.WriteLine(msg.Values);
                                string[] info = msg.Values.Split("@");
                                List<string> information = info.ToList();
                                List<List<int>> occuaptions = new List<List<int>>();                                
                                int rideNumber = 0;

                                Console.WriteLine(info[0]);
                                Console.WriteLine(information[0]);
                                if (Int32.TryParse(information[0], out rideNumber))
                                {
                                    Console.WriteLine("got rideNumber");
                                    information.RemoveAt(0);

                                    foreach (Track track in tracks)
                                    {
                                        if (track.RideNumber == rideNumber)
                                        {
                                            Console.WriteLine("found track");
                                            foreach (string infos in information)
                                            {
                                                List<int> occu = infos.Split(',').Select(int.Parse).ToList();
                                                Console.WriteLine(occu);
                                                occuaptions.Add(occu);
                                            }

                                            List<int> occupation = new List<int>();
                                            foreach (int trainUnit in fakeApi.GetTrainUnits(rideNumber))
                                            {
                                                Console.WriteLine(trainUnit);
                                                foreach (List<int> occu in occuaptions)
                                                {
                                                    if (occu[0] == trainUnit)
                                                    {
                                                        Console.WriteLine("updating occupation");
                                                        for (int i = 1; i < occu.Count; i++)
                                                        {
                                                            Console.WriteLine(occu[i]);
                                                            occupation.Add(occu[i]);
                                                        }
                                                    }
                                                }
                                            }

                                            track.UpdateLamps(occupation);
                                            track.TrackState = TrackState.UpdatedOccupation;
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
            switch (connectionState)
            {
                case ConnectionState.Identify:
                    Console.WriteLine("Connecting to server");
                    connection.SendMessage("CONNECT:STATION");
                    connectionState = ConnectionState.Identifying;
                    break;

                case ConnectionState.Register:
                    Console.WriteLine("Identifying as a station");
                    connection.SendMessage("IAM:" + StationName);
                    connectionState = ConnectionState.Registering;
                    break;

                case ConnectionState.Connected:
                    foreach (Track track in tracks)
                    {
                        switch (track.TrackState)
                        {
                            case TrackState.NoOccupation:
                                int rideNumber = fakeApi.GetRideByTrackId(track.TrackId);
                                if (rideNumber > 0)
                                {
                                    Console.WriteLine("Asking for occupation of ride: " + rideNumber);
                                    track.RideNumber = rideNumber;
                                    connection.SendMessage("GETOCCUPATION:" + rideNumber);
                                    track.TrackState = TrackState.RequestedOccupation;
                                    track.LastUpdate = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                                }
                                break;

                            case TrackState.UpdatedOccupation:
                                long yeet = DateTimeOffset.Now.ToUnixTimeMilliseconds() - track.LastUpdate;
                                if(yeet > 30000)
                                {
                                    track.TrackState = TrackState.NoOccupation;
                                }

                                break;
                        }
                    }
                    break;
            }
        }
    }
}