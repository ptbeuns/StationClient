using System;

namespace StationClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigLoader ConfigLoader = new ConfigLoader("StationConfig.json");
            ConfigLoader.OpenConfig();
            Station station = ConfigLoader.LoadStationFromConfig();
            station.LoadApi();
            station.ConnectToServer();

            while(true)
            {
                station.UpdateTracks();
                station.ReceiveMessages();
            }
        }
    }
}
