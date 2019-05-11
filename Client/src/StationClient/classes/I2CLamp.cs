namespace StationClient
{
    public class I2CLamp
    {
        public int LampSlaveId{get; private set;}

        public I2CLamp(int lampSlaveId)
        {
            LampSlaveId = lampSlaveId;
        }

        public void SetLampColor(int occupation)
        {
            //TODO: implement I2C
        }
    }
}