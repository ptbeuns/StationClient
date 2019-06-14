using System;

namespace StationClient
{
    class Program
    {
        static void Main(string[] args)
        {
            /*ConfigLoader ConfigLoader = new ConfigLoader("StationConfig.json");
            ConfigLoader.OpenConfig();
            Station station = ConfigLoader.LoadStationFromConfig();
            station.LoadApi();
            station.ConnectToServer();*/

            I2CLamp lamp1 = new I2CLamp(8);
            I2CLamp lamp2 = new I2CLamp(9);
            I2CLamp lamp3 = new I2CLamp(10);
            I2CLamp lamp4 = new I2CLamp(11);

            lamp1.SetLampColor(0);
            lamp2.SetLampColor(25);
            lamp3.SetLampColor(75);
            lamp4.SetLampColor(100);

            /*while (true)
            {
                station.UpdateTracks();
                station.ReceiveMessages();
            }*/
        }
    }
}
