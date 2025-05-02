using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

internal class Program
{
    public static List<TcpClient> tcpClients = new List<TcpClient>();
    private static async Task Start(TcpClient client)
    {
        Console.WriteLine("Connected!!!");

        try
        {
            NetworkStream ns = client.GetStream();

            StreamWriter sw = new StreamWriter(ns);
            StreamReader sr = new StreamReader(ns);

            while (true)
            {
                string fullMessage = sr.ReadLine()!;
                
                int Index = fullMessage.IndexOf(':');
                if (Index == -1)
                    break;

                var parts = fullMessage.Split(':', 2);
                if (parts.Length != 2)
                {
                    return;
                }

                string userName = parts[0];
                string message = parts[1];

                Console.WriteLine($"{DateTime.Now.ToLongTimeString()} {userName} :: {message} from -- {client.Client.RemoteEndPoint}");

                if (message == "$<close>")
                {
                    tcpClients.Remove(client);
                    sw.Close();
                    sr.Close();
                    ns.Close();
                    return;
                }

                foreach (var tcpClient in tcpClients)
                {
                    try
                    {
                        if (tcpClient.Connected)
                        {
                            NetworkStream clientStream = tcpClient.GetStream();
                            StreamWriter sw_ = new StreamWriter(clientStream) { AutoFlush = true };
                            sw_.WriteLine($"{userName}:{message}");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Помилка при надсиланні до клієнта: {e.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    

    private static void Main(string[] args)
    {

        IPEndPoint IpPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4040);
        TcpListener server = new TcpListener(IpPoint);

        try
        {
            server.Start();

            Console.WriteLine("Server started! Waiting for connection .... ");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                if(tcpClients.Contains(client))
                    continue;

                tcpClients.Add(client);
                Task.Run(() => Start(client));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}