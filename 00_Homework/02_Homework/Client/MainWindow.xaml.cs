using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
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
    ///
    public partial class MainWindow : Window
    {
        const string serverAddress = "127.0.0.1";
        const int port = 4040;
        IPEndPoint server;
        UdpClient client;
        ObservableCollection<MessageInfo> messages;

        string UserName;

        public MainWindow(string userName)
        {
            InitializeComponent();
            server = new IPEndPoint(IPAddress.Parse(serverAddress), port);
            messages = new ObservableCollection<MessageInfo>();
            client = new UdpClient();
            this.DataContext = messages;
            UserName = userName;
        }

        private void SendBtn(object sender, RoutedEventArgs e)
        {
            string message = msgTextBox.Text;
            msgTextBox.Text = "";
            if(string.IsNullOrWhiteSpace(message))
                return;

            SendMessage(message);
        }
        private void msgTextBoxEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendBtn(sender, e);
            }
        }
        private async void SendMessage(string message)
        {
            string fullMessage = $"{UserName}:{message}";
            byte[] data = Encoding.Unicode.GetBytes(fullMessage);
            await client.SendAsync(data, data.Length, server);
        }
        private async void Listener(bool isListening)
        {
            while (isListening)
            {
                var data = await client.ReceiveAsync();
                string fullMessage = Encoding.Unicode.GetString(data.Buffer);

                int Index = fullMessage.IndexOf(':');
                if (Index == -1)
                    continue;

                string userName_chat = fullMessage.Substring(0, Index);
                string message = fullMessage.Substring(Index + 1);

                messages.Add(new MessageInfo((userName_chat + " :: "), message));
            }
        }
        private void JoinBtn(object sender, RoutedEventArgs e)
        {
            SendMessage("$<join>");
            Listener(true);
        }
        private void LeaveBtn(object sender, RoutedEventArgs e)
        {
            SendMessage("$<leave>");
            ClearBtn(sender, e);
            Listener(false);
        }
        private void ClearBtn(object sender, RoutedEventArgs e)
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
        public MessageInfo(string userName,string message)
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