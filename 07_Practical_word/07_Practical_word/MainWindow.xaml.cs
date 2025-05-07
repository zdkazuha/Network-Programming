using MailKit;
using MailKit.Net.Imap;
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

namespace _07_Practical_word
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public partial class MainWindow : Window
    {
        Dictionary<string, string> foldersName;
        List<MimeKit.MimeMessage> Messages;
        string username;
        string password;

        string folderName_;
        MessageInfo messageInfo;

        public MainWindow(string userName,string Password)
        {
            InitializeComponent();

            Console.OutputEncoding = Encoding.UTF8;

            foldersName = new Dictionary<string, string>();
            Messages = new List<MimeKit.MimeMessage>();
            username = userName;
            password = Password;
            messageInfo = new MessageInfo("", "", "");

            ViewFolders();

            listFolder.SelectionChanged += ViewMessages;
            listMessages.SelectionChanged += ViewMessageDetails;
        }


        public void ViewFolders()
        {
            using(var client = new ImapClient())
            {
                client.Connect("imap.gmail.com", 993, MailKit.Security.SecureSocketOptions.SslOnConnect);
                client.Authenticate(username, password);

                var folders = client.GetFolders(client.PersonalNamespaces[0]);
                foreach (var folder in folders)
                {
                    if (folder.Name == "INBOX" || folder.Name == "[Gmail]")
                        continue;
                    listFolder.Items.Add(folder.Name);
                    foldersName.Add(folder.Name, folder.FullName);
                }
            }
        }

        public async void ViewMessages(object sender, SelectionChangedEventArgs e)
        {
            if (listFolder.SelectedItem == null)
                return;

            foreach (var item in foldersName)
            {
                if(item.Key == listFolder.SelectedItem.ToString())
                {
                    folderName_ = item.Value;
                }
            }

            using (var client = new ImapClient())
            {
                client.Connect("imap.gmail.com", 993, MailKit.Security.SecureSocketOptions.SslOnConnect);
                client.Authenticate(username, password);

                var folder = await client.GetFolderAsync(folderName_);
                await folder.OpenAsync(FolderAccess.ReadWrite);

                listMessages.Items.Clear();
                Messages.Clear();

                var mes = await folder.SearchAsync(MailKit.Search.SearchQuery.All);
                foreach (var m in mes)
                {
                    var message = await folder.GetMessageAsync(m);
                    listMessages.Items.Add($"{message.Date}: {message.Subject}");
                    Messages.Add(message);
                }
            }
        }
        private void ViewMessageDetails(object sender, SelectionChangedEventArgs e)
        {
            if (listMessages.SelectedItem == null)
                return;

            int index = listMessages.SelectedIndex;
            if (index >= 0 && index < Messages.Count)
            {
                var message = Messages[index];
                messageInfo = new MessageInfo(message.From.ToString(), message.To.ToString(), message.Subject.ToString());
                messageInfo.Show();
            }
        }
    }
}