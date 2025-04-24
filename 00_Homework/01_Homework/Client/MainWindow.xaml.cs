using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string address = "127.0.0.1";
        static int port = 9090;

        IPEndPoint endPoint;
        EndPoint remoteEP;
        Socket socket;

        string fileName = "../../../streets.txt";

        public MainWindow()
        {
            InitializeComponent();
            
            endPoint = new IPEndPoint(IPAddress.Parse(address), port);
            remoteEP = new IPEndPoint(IPAddress.Any, 0);

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 0));

            Listening();
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Index.Text))
            {
                MessageBox.Show("Please enter a zip code.");
                return;
            }

            ListIndex.Items.Clear();

            string zipCode = Index.Text.Trim();
            byte[] bytes = Encoding.Unicode.GetBytes(zipCode);

            socket.SendTo(bytes, endPoint);
        }

        private async void Listening()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    byte[] data = new byte[1024];
                    EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                    int received = socket.ReceiveFrom(data, ref remoteEP);

                    string response = Encoding.Unicode.GetString(data, 0, received);
                    
                    Dispatcher.Invoke(() =>
                    {
                        ListIndex.Items.Add(response);
                    });
                }
            });
        }

        private void Index_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Start(sender, e);
            }
        }

        private void SaveToFile(object sender, RoutedEventArgs e)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                if(ListIndex.Items.Count == 0)
                {
                    MessageBox.Show("No data to save.");
                    return;
                }
                foreach (var item in ListIndex.Items)
                {
                    sw.WriteLine(item.ToString());
                }
                MessageBox.Show("Data saved to file.");
            }
        }
    }
}