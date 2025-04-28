using System.Collections.ObjectModel;
using System.IO;
using System.Net.Sockets;
using System.Net;
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
using System.Configuration;

namespace _03_Homework
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IPEndPoint server;
        TcpClient client;
        NetworkStream ns = null;
        ObservableCollection<MessageInfo> messages;

        StreamWriter sw;
        StreamReader sr;
        public MainWindow()
        {
            InitializeComponent();
            string address = ConfigurationManager.AppSettings["serverAddress"]!;
            short port = short.Parse(ConfigurationManager.AppSettings["serverPort"]!);
            server = new IPEndPoint(IPAddress.Parse(address), port);
            messages = new ObservableCollection<MessageInfo>();
            this.DataContext = messages;

            ConnectedBtn();
        }

        private void SendBtn(object sender, RoutedEventArgs e)
        {
            string message = MessageTextBox.Text;
            MessageTextBox.Text = "";
            if (string.IsNullOrWhiteSpace(message))
                return;

            if (sw == null)
                return;

            sw.WriteLine(message);

            sw.Flush();
        }

        private void MessageTextBoxEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendBtn(sender, e);
            }
        }
        private void ConnectedBtn()
        {
            try
            {
                client = new TcpClient();
                client.Connect(server);
                ns = client.GetStream();

                sw = new StreamWriter(ns);
                sr = new StreamReader(ns);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Listener();
        }
        private async void Listener()
        {
            while (true)
            {
                try
                {
                    string? message = await sr.ReadLineAsync();
                    if (message != null)
                        messages.Add(new MessageInfo(message));
                }
                catch (Exception ex)
                {
                    break;
                }
            }
        }

        private void CloseChat(object sender, RoutedEventArgs e)
        {
            sw.WriteLine("$<close>");
            sw.Flush();

            sw.Close();
            ns.Close();
            client.Close();

            sw = null;

            this.Close();
        }
    }
    public class MessageInfo
    {
        public string Message { get; set; }
        private DateTime time;
        public string Time => time.ToLongTimeString();
        public MessageInfo(string message)
        {
            Message = message;
            time = DateTime.Now;
        }
        public override string ToString()
        {
            return $"{Message,-20} {Time} ";
        }
    }
}