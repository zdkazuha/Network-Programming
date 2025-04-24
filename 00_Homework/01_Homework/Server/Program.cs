using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Net.Http.Headers;

internal class Program
{
    static string address = "127.0.0.1";
    static int port = 9090;

    private static void Main(string[] args)
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(address), port);
        EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
        
        Socket listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        List<Street> streets = new List<Street>
        {
            new Street { ZipCode = "01001", StreetName = "Shevchenka Street" },
            new Street { ZipCode = "01002", StreetName = "Franko Avenue" },
            new Street { ZipCode = "01003", StreetName = "Bandera Street" },
            new Street { ZipCode = "01004", StreetName = "Hrushevskoho Street" },
            new Street { ZipCode = "01005", StreetName = "Khmelnytskoho Street" },
            new Street { ZipCode = "01006", StreetName = "Soborna Street" },
            new Street { ZipCode = "01007", StreetName = "Mazepy Street" },
            new Street { ZipCode = "01008", StreetName = "Voznesenska Street" },
            new Street { ZipCode = "01009", StreetName = "Nezalezhnosti Avenue" },
            new Street { ZipCode = "01000", StreetName = "Dniprovska Street" },
            new Street { ZipCode = "01001", StreetName = "Lermontova Street" },
            new Street { ZipCode = "01002", StreetName = "Pushkina Street" },
            new Street { ZipCode = "01003", StreetName = "Kotsiubynskoho Street" },
            new Street { ZipCode = "01004", StreetName = "Sadova Street" },
            new Street { ZipCode = "01005", StreetName = "Lisova Street" },
            new Street { ZipCode = "01006", StreetName = "Zhovtneva Street" },
            new Street { ZipCode = "01007", StreetName = "Peremohy Avenue" },
            new Street { ZipCode = "01008", StreetName = "Vynnychenka Street" },
            new Street { ZipCode = "01009", StreetName = "Yaroslaviv Val" },
            new Street { ZipCode = "01000", StreetName = "Kyivska Street" }
        };

        byte[] bytes = Encoding.Unicode.GetBytes("Not street found");

        try
        {
            listeningSocket.Bind(endPoint);
            Console.WriteLine("Server started. Waiting for messages...");
            while (true)
            {
                byte[] data = new byte[1024];
                int receivedBytes = listeningSocket.ReceiveFrom(data, ref remoteEP);

                string message = Encoding.Unicode.GetString(data, 0, receivedBytes).Trim();
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()} :: {message} from {remoteEP}");

                List<Street> response = streets.FindAll(r => r.ZipCode == message);

                if (!response.Any())
                {
                    listeningSocket.SendTo(bytes, remoteEP);
                }
                else
                {
                    foreach (var item in response)
                    {
                        byte[] street = Encoding.Unicode.GetBytes(item.ToString());
                        listeningSocket.SendTo(street, remoteEP);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    class Street
    {
        public string ZipCode { get; set; }
        public string StreetName { get; set; }

        public override string ToString()
        {
            return $"ZipCode :: {ZipCode} - Street Name ::  {StreetName}";
        }
    }
}