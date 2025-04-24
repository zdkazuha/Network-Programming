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
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        try
        {
            string message = "";
            UdpClient client = new UdpClient();
            while (message != "end")
            {
                Console.Write("enter s message :: ");
                message = Console.ReadLine()!;
                byte[] data = Encoding.Unicode.GetBytes(message);

                client.Send(data,data.Length,endPoint);
                ///////////////////////////
                
                string response = "";

                data = client.Receive(ref remoteEndPoint);
                response = Encoding.Unicode.GetString(data);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(response);
                Console.ResetColor();
            }
            client.Close();
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
    }
}