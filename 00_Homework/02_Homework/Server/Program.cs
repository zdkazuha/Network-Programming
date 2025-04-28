using System.IO.MemoryMappedFiles;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Serialization;

public class ChatServer
{
    const int port = 4040;
    const string JOIN_CMD = "$<join>";
    const string LEAVE_CMD = "$<leave>";

    UdpClient server;
    IPEndPoint client = null;
    List<IPEndPoint> members;

    public ChatServer()
    {
        server = new UdpClient(port);
        members = new List<IPEndPoint>();
    }
    private void AddMember(IPEndPoint member)
    {
        if(!members.Contains(member))
            members.Add(member);

        Console.WriteLine($"Member was added ---- {members.Count}");
    }
    private async void SendAllMember(string userName, string message)
    {
        string fullMessage = $"{userName}:{message}";
        byte[] data = Encoding.Unicode.GetBytes(fullMessage);

        foreach (var item in members)
        {
            await server.SendAsync(data, data.Length, item);
        }
    }


    public void Start()
    {
        while (true)
        {
            byte[] data = server.Receive(ref client);
            string fullMessage = Encoding.Unicode.GetString(data);

            int Index = fullMessage.IndexOf(':');
            if (Index == -1)
                continue;

            var parts = fullMessage.Split(':', 2);
            if (parts.Length != 2)
            {
                return;
            }

            string userName = parts[0];
            string message = parts[1];

            switch (message)
            {
                case JOIN_CMD:
                    AddMember(client);
                    break;
                case LEAVE_CMD:
                    members.Remove(client);
                    Console.WriteLine($"Member was removed ---- {members.Count}");
                    break;
                default:
                    Console.WriteLine($"{DateTime.Now.ToLongTimeString()} {userName} :: {message} from -- {client}");
                    if (!members.Contains(client))
                        break;

                    SendAllMember(userName,message);
                    break;
            }
        }
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        ChatServer chat = new ChatServer();
        chat.Start();
    }
}