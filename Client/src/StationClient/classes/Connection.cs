using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

namespace StationClient
{
    public class Connection
    {
        private Socket connectionSocket;
        private IPEndPoint connectionEndPoint;

        public Connection(IPAddress ip, IPEndPoint endpoint)
        {
            connectionEndPoint = endpoint;
            connectionSocket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect()
        {
            connectionSocket.Connect(connectionEndPoint);
        }

        public List<Message> ReceiveMessage()
        {
            List<Message> messages = new List<Message>();
            byte[] data = new byte[1024];
            string message = "";

            try
            {
                if (connectionSocket.Poll(1000, SelectMode.SelectRead))
                {
                    int bytesRec = connectionSocket.Receive(data);
                    message += Encoding.ASCII.GetString(data, 0, bytesRec);

                    for (int i = 0; i < message.Length; i++)
                    {
                        if (message[i] == '&')
                        {
                            string msg = "";

                            for (int j = i + 1; j < message.Length; j++)
                            {
                                if (message[j] == '%')
                                {
                                    messages.Add(new Message(msg));
                                    i = j;
                                    break;
                                }
                                else
                                {
                                    msg += message[j];
                                }
                            }
                        }
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
                connectionSocket.Close();
                connectionSocket.Dispose();
            }

            return messages;
        }

        public void SendMessage(string msg)
        {
            byte[] message = Encoding.ASCII.GetBytes("&" + msg + "%");

            try
            {
                connectionSocket.Send(message);
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
                connectionSocket.Close();
                connectionSocket.Dispose();
            }
        }

        public void SendACK()
        {
            SendMessage("ACK");
        }

        public void SendNACK()
        {
            SendMessage("NACK");
        }
    }
}