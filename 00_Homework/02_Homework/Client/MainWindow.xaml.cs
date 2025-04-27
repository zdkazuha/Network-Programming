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
            byte[] data_UserName = Encoding.Unicode.GetBytes(UserName);
            await client.SendAsync(data_UserName, data_UserName.Length, server);

            byte[] data_Message = Encoding.Unicode.GetBytes(message);
            await client.SendAsync(data_Message, data_Message.Length, server);
        }
        private async void Listener()
        {
            while(true)
            {
                var data_UserName = await client.ReceiveAsync();
                string userName_chat = Encoding.Unicode.GetString(data_UserName.Buffer);

                var data_Message = await client.ReceiveAsync();
                string message = Encoding.Unicode.GetString(data_Message.Buffer);

                messages.Add(new MessageInfo((userName_chat + " :: "),message));
            }
        }
        private void JoinBtn(object sender, RoutedEventArgs e)
        {
            SendMessage("$<join>");
            Listener();
        }
        private void LeaveBtn(object sender, RoutedEventArgs e)
        {
            SendMessage("$<leave>");
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