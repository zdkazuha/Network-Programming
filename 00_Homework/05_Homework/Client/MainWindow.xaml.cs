using System.Collections.ObjectModel;
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
using System.IO;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    public partial class MainWindow : Window
    {

        IPEndPoint server;
        TcpClient client;
        NetworkStream ns = null;
        public static ObservableCollection<MessageInfo> messages;

        StreamWriter sw;
        StreamReader sr;

        string UserName = "";
        string CLOSE = "$<close>";

        public static bool isListener = true;

        public MainWindow(string UserName_Chat)
        {
            InitializeComponent();

            string address = ConfigurationManager.AppSettings["serverAddress"]!;
            short port = short.Parse(ConfigurationManager.AppSettings["serverPort"]!);

            server = new IPEndPoint(IPAddress.Parse(address), port);
            messages = new ObservableCollection<MessageInfo>();

            this.DataContext = messages;
            UserName = UserName_Chat;
        }

        private void msgTextBoxEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendBtn(sender, e);
            }
        }
        private void SendBtn(object sender, RoutedEventArgs e)
        {
            string Text = msgTextBox.Text;
            msgTextBox.Text = "";
            if(string.IsNullOrWhiteSpace(Text))
            {
                MessageBox.Show("Будь ласка введіть повідомлення!");
                return;
            }
            if (sw == null)
                return;

            string message = $"{UserName}:{Text}";

            sw.WriteLine(message);
            sw.Flush();

        }
        private void ConnectedBtn(object sender, RoutedEventArgs e)
        {
            try
            {
                if(sw != null)
                {
                    MessageBox.Show("Ви вже підключені до сервера!");
                    return;
                }

                client = new TcpClient();
                client.Connect(server);
                ns = client.GetStream();

                sw = new StreamWriter(ns);
                sr = new StreamReader(ns);

                isListener = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Listener();
        }
        private async void Listener()
        {
            while (isListener)
            {
                try
                {
                    string? fullMessage = await sr.ReadLineAsync();
                    if (fullMessage != null)
                    {
                        int Index = fullMessage.IndexOf(':');
                        if (Index == -1)
                            return;

                        var parts = fullMessage.Split(':', 2);
                        if (parts.Length != 2)
                        {
                            return;
                        }

                        string userName = parts[0];
                        string message = parts[1];


                        messages.Add(new MessageInfo((userName + " :: "), message));

                        if (userName == "Сервер" && message == "Сервер переповнений!")
                        {
                            sw = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                     //
                }
            }
        }
        private void DisconnectedBtn(object sender, RoutedEventArgs e)
        {
            
            try
            {
                if (sw == null)
                {
                    MessageBox.Show("Ви ще не приєдналися до сервера!");
                    return;
                }
                sw.WriteLine($"{UserName}:{CLOSE}");
                sw.Flush();

                client.Close();
                ns.Close();

                sw.Close();
                sr.Close();

                isListener = false;

                sw = null;

                ClearBtn(sender, e);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void ClearBtn(object sender, RoutedEventArgs e)
        {
            messages.Clear();
        }
    }

    public class MessageInfo
    {
        public string UserName { get; set; }
        public string Message { get; set; }
        private DateTime time;
        public string Time => time.ToLongTimeString();
        public MessageInfo(string userName ,string message)
        {
            UserName = userName;
            Message = message;
            time = DateTime.Now;
        }
        public override string ToString()
        {
            return $"{Message,-20} {Time} ";
        }
    }

}