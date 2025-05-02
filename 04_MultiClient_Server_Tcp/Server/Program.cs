using System.Net.Sockets;
using System.Net;
using System.Numerics;

internal class Program
{
    private static void Main(string[] args)
    {
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
        TcpListener listener = new TcpListener(ipPoint);
        try
        {
            listener.Start();

            Console.WriteLine("Server started! Waiting for connection .... ");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Task.Run(() => ServerClient(client));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            listener.Stop();
        }
    }

    private static void ServerClient(TcpClient client)
    {
        try
        {
            NetworkStream ns = client.GetStream();
            StreamReader sr = new StreamReader(ns);
            string message = sr.ReadLine()!;

            Console.WriteLine($"{client.Client.RemoteEndPoint} - {message} at {DateTime.Now.ToLongTimeString()}");


            StreamWriter sw = new StreamWriter(ns);
            long number = long.Parse(message);

            sw.WriteLine(GetFactorial(number));

            sw.Close();
            sr.Close();
            ns.Close();

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally { client.Close(); }
    }

    private static BigInteger GetFactorial(long number)
    {
        BigInteger res = 1;
        for (int i = 2; i <= number; i++)
        {
            res *= (ulong)i;
            //Thread.Sleep(500);
        }
        return res;
    }
}