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
        EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        Socket listensSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        try
        {
            listensSocket.Bind(endPoint);
            Console.WriteLine("Server Started! Watin connection");
            while (true)
            {
                byte[] data = new byte[1024];
                listensSocket.ReceiveFrom(data, ref remoteEndPoint);

                string message = Encoding.Unicode.GetString(data);
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()} :: {message} from {remoteEndPoint}");

                //////
                message = "Message was send";
                data = Encoding.Unicode.GetBytes(message);
                listensSocket.SendTo(data, remoteEndPoint);


            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }

        listensSocket.Shutdown(SocketShutdown.Both);
        listensSocket.Close();
    }
}