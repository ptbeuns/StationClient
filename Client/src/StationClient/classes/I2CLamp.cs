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
            i2c.Open(lampSlaveId);
        }

        public void SetLampColor(int occupation)
        {
            Console.WriteLine(occupation);
            byte[] intBytes = BitConverter.GetBytes(occupation);
            Console.WriteLine(intBytes);
            i2c.Write(intBytes);
        }
    }
}