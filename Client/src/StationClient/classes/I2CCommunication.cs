using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace StationClient
{
    class I2CCommunication
    {
        private static int OPEN_READ_WRITE = 2;
        private static int I2C_SLAVE = 0x0703;

        [DllImport("libc.so.6", EntryPoint = "open")]
        private static extern int Open(string fileName, int mode);

        [DllImport("libc.so.6", EntryPoint="ioctl", SetLastError = true)]
        private static extern int Ioctl(int fd, int request, int data);

        [DllImport("libc.so.6", EntryPoint="read", SetLastError = true)]
        private static extern int Read(int handle, byte[] data, int length);

        
        [DllImport("libc.so.6", EntryPoint="write", SetLastError = true)]
        private static extern void Write(int handle, byte[] data, int length);
        static int I2CBushandle;
        static int DeviceReturnCode;

        public void Open(int adress)
        {
            I2CBushandle = Open("/dev/i2c-1", OPEN_READ_WRITE);
            DeviceReturnCode = Ioctl(I2CBushandle, I2C_SLAVE, adress);
        }

        public byte[] Read(int bytesToRead)
        {
            byte[] buffer = new byte[bytesToRead];
            Read(I2CBushandle, buffer, buffer.Length);
            Thread.Sleep(100);
            return buffer;
        }

        public void Write(byte[] data)
        {
            Write(I2CBushandle, data, data.Length);
            Thread.Sleep(100);
        }
    }
}