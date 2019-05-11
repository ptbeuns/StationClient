namespace StationClient
{
    public class Message
    {
        public string Command { get; private set; }
        public string Values { get; private set; }

        public Message(string message)
        {
            string[] msg = message.Split(':');
            Command = msg[0];
            if (msg.Length > 1)
            {
                Values = msg[1];
            }
        }

        public Message(string command, string values)
        {
            Command = command;
            Values = values;
        }
    }
}