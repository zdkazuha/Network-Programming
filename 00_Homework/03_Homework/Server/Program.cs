using Server;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Serialization;

public class ChatServer
{
    const int port = 4040;

    TcpListener server;
    DictionaryChat dictionary;
    public ChatServer()
    {
        server = new TcpListener(new IPEndPoint(IPAddress.Parse("127.0.0.1"), port));
        dictionary = new DictionaryChat();
    }

    public void Start()
    {
        server.Start();
        Console.WriteLine("Waiting for connection");
        TcpClient client = server.AcceptTcpClient();

        Console.WriteLine("Connected!!!");
        NetworkStream ns = client.GetStream();

        StreamWriter sw = new StreamWriter(ns);
        StreamReader sr = new StreamReader(ns);

        while (true)
        {

            string message = sr.ReadLine()!;

            if (message == "$<close>")
            {
                ns.Close();
                server?.Stop();
                break;
            }

            string response = dictionary.GetResponse(message);
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()} -- {message} from -- {client.Client.LocalEndPoint}");

            sw.WriteLine(response);
            sw.Flush();
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