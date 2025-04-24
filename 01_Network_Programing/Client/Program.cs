using System.Net;
using System.Net.Sockets;
using System.Text;

internal class Program
{
    static string address = "127.0.0.1";
    static int port = 4040; // 1000 - 65000

    private static void Main(string[] args)
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(address), port);
        EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        try
        {
            string message = "";
            while (message != "end")
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                Console.Write("enter s message :: ");
                message = Console.ReadLine()!;
                byte[] data = Encoding.Unicode.GetBytes(message);

                //socket.Connect(endPoint);
                //socket.Send(data);

                socket.SendTo(data,endPoint);
                //socket.Close();
                string response = "";
                do
                {
                    data = new byte[1024]; // 1kb
                    socket.ReceiveFrom(data, ref remoteEndPoint);

                    response += Encoding.Unicode.GetString(data);

                } while (socket.Available > 0);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(response);
                Console.ResetColor();
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
    }
}