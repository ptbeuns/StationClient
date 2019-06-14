using System;
using System.Text;

namespace StationClient
{
    public class I2CLamp
    {
        private I2CCommunication i2c;
        public int LampSlaveId { get; private set; }

        public I2CLamp(int lampSlaveId)
        {
            LampSlaveId = lampSlaveId;
            i2c = new I2CCommunication();
        }

        public void SetLampColor(int occupation)
        {
            i2c.Open(LampSlaveId);

            Console.WriteLine(occupation);
            Console.WriteLine(LampSlaveId);
            byte[] data = BitConverter.GetBytes(occupation);
            foreach(byte b in data)
            {
                Console.WriteLine(b);
            }
            
            i2c.Write(data);

            i2c.Close();
        }
    }
}