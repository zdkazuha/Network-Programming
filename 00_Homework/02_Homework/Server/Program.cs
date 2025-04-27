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
    private async void SendAllMember(string userName,string message)
    {
        byte[] data_UserName = Encoding.Unicode.GetBytes(userName);
        byte[] data_Message = Encoding.Unicode.GetBytes(message);
        foreach (var item in members)
        {
            await server.SendAsync(data_UserName, data_UserName.Length, item);
            await server.SendAsync(data_Message, data_Message.Length, item);
        }
    }

    public void Start()
    {
        while (true)
        {
            byte[] data_UserName = server.Receive(ref client);
            string userName = Encoding.Unicode.GetString(data_UserName);

            byte[] data_Message = server.Receive(ref client);
            string message = Encoding.Unicode.GetString(data_Message);

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