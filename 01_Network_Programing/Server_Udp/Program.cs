using System.Net;
using System.Net.Sockets;
using System.Text;

internal class Program
{
    static string address = "127.0.0.1";
    static int port = 4040; // 1000 - 65000
    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.Unicode;
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(address), port);
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        UdpClient listens = new UdpClient(endPoint);

        try
        {
            Console.WriteLine("Server Started! Watin connection");
            byte[] data;
            while (true)
            {
                data = listens.Receive(ref remoteEndPoint);

                string message = Encoding.Unicode.GetString(data);
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()} :: {message} from {remoteEndPoint}");

                //////
                message = "Message was send";
                data = Encoding.Unicode.GetBytes(message);
                listens.Send(data, data.Length, remoteEndPoint);


            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
        listens.Close();
    }
}