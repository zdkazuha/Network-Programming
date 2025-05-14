using MailKit;
using MailKit.Net.Imap;
using MimeKit;
using System.ComponentModel;
using System.Data;
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

        int count = 0;
        public MainWindow(string userName, string Password)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Console.OutputEncoding = Encoding.UTF8;

            foldersName = new Dictionary<string, string>();
            Messages = new List<MimeKit.MimeMessage>();
            username = userName;
            password = Password;
            count = 0;

            ViewFolders();

            listFolder.SelectionChanged += ViewMessages;
            searchBox.TextChanged += SearchMessage;
        }

        public void ViewFolders()
        {
            listFolder.Items.Clear();
            foldersName.Clear();

            using (var client = new ImapClient())
            {
                client.Connect("imap.gmail.com", 993, MailKit.Security.SecureSocketOptions.SslOnConnect);
                client.Authenticate(username, password);

                var folders = client.GetFolders(client.PersonalNamespaces[0]);
                foreach (var folder in folders)
                {
                    if ( folder.Name == "[Gmail]")
                        continue;
                    if (folder.Name == "INBOX")
                    {
                        listFolder.Items.Add("Вхідні");
                        foldersName.Add("Вхідні", folder.FullName);
                        continue;
                    }
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
                if (item.Key == listFolder.SelectedItem.ToString())
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
                SortMessage(null, null);
            }
        }
        private void OpenMessageWindowBtn(object sender, RoutedEventArgs e)
        {
            if (listMessages.SelectedItem == null)
                return;

            int index = listMessages.SelectedIndex;
            var message = Messages[index];
            if (index >= 0 && index < Messages.Count)
                message = Messages[index];
            else
                return;

            MessageWindow messageWindow = new MessageWindow(username, password, message);
            messageWindow.Show();
        }

        private void MoveToSpamMessage(object sender, RoutedEventArgs e)
        {
            if(listFolder.SelectedItem == null)
                return;

            foreach (var item in foldersName)
            {
                if (item.Key == listFolder.SelectedItem.ToString())
                    folderName_ = item.Value;
            }

            using (var client = new ImapClient())
            {
                try
                {

                client.Connect("imap.gmail.com", 993, MailKit.Security.SecureSocketOptions.SslOnConnect);
                client.Authenticate(username, password);

                var folder = client.GetFolder(folderName_);
                folder.Open(MailKit.FolderAccess.ReadWrite);
                var message = Messages[listMessages.SelectedIndex];

                Messages.RemoveAt(listMessages.SelectedIndex);

                folder.MoveTo(listMessages.SelectedIndex, client.GetFolder(SpecialFolder.Junk));
                folder.AddFlags(listMessages.SelectedIndex, MessageFlags.Deleted, true);
                folder.Expunge();

                MessageBox.Show("Повідомлення переміщено в спам.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Повідомлення не переміщено в спам :: {ex.Message}");
                }
            }
        }
        private void DeleteMessage(object sender, RoutedEventArgs e)
        {
            if (listFolder.SelectedItem == null)
                return;

            foreach (var item in foldersName)
            {
                if (item.Key == listFolder.SelectedItem.ToString())
                    folderName_ = item.Value;
            }
            using (var client = new ImapClient())
            {
                try
                {

                    client.Connect("imap.gmail.com", 993, MailKit.Security.SecureSocketOptions.SslOnConnect);
                    client.Authenticate(username, password);

                    var folder = client.GetFolder(folderName_);
                    folder.Open(MailKit.FolderAccess.ReadWrite);
                    var message = Messages[listMessages.SelectedIndex];

                    Messages.RemoveAt(listMessages.SelectedIndex);

                    folder.MoveTo(listMessages.SelectedIndex, client.GetFolder(SpecialFolder.Trash));
                    folder.AddFlags(listMessages.SelectedIndex, MessageFlags.Deleted, true);
                    folder.Expunge();

                    MessageBox.Show("Повідомлення переміщено в видаленно.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Повідомлення не переміщено в корзину :: {ex.Message}");
                }
            }
        }
        private void SortMessage(object sender, RoutedEventArgs e)
        {
            Messages = Messages.OrderByDescending(m => m.Date).ToList();

            listMessages.Items.Clear();

            foreach (var message in Messages)
            {
                listMessages.Items.Add($"{message.Date}: {message.Subject}");
            }
        }

        public async void SearchMessage(object sender, RoutedEventArgs e)
        {
            if (listFolder.SelectedItem == null)
                return;

            foreach (var item in foldersName)
            {
                if (item.Key == listFolder.SelectedItem.ToString())
                    folderName_ = item.Value;
            }

            using (var client = new ImapClient())
            {
                try
                {
                    await client.ConnectAsync("imap.gmail.com", 993, MailKit.Security.SecureSocketOptions.SslOnConnect);
                    await client.AuthenticateAsync(username, password);

                    var folder = client.GetFolder(folderName_);
                    await folder.OpenAsync(MailKit.FolderAccess.ReadWrite);

                    var searchQuery = MailKit.Search.SearchQuery.SubjectContains(searchBox.Text);

                    var results = await folder.SearchAsync(searchQuery);

                    listMessages.Items.Clear();
                    Messages.Clear();

                    foreach (var result in results)
                    {
                        var message = await folder.GetMessageAsync(result);
                        listMessages.Items.Add($"{message.Date}: {message.Subject}");
                        Messages.Add(message);
                    }
                    SortMessage(null, null);
                }
                catch (Exception ex)
                {
                    
                }
            }
        }
        private void searchBoxEnter(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                SearchMessage(sender, e);
            }
        }

        private async void RenameFolder(string oldFolderName, string newFolderName)
        {
            using (var client = new ImapClient())
            {
                try
                {
                    await client.ConnectAsync("imap.gmail.com", 993, MailKit.Security.SecureSocketOptions.SslOnConnect);
                    await client.AuthenticateAsync(username, password);

                    var personal = client.PersonalNamespaces[0];
                    var rootFolder = client.GetFolder(personal);

                    var oldFolder = await client.GetFolderAsync(oldFolderName);
                    await oldFolder.OpenAsync(FolderAccess.ReadWrite);

                    var newFolder = await rootFolder.CreateAsync(newFolderName, true);

                    var uids = await oldFolder.SearchAsync(MailKit.Search.SearchQuery.All);
                    foreach (var uid in uids)
                    {
                        await oldFolder.MoveToAsync(uid, newFolder);
                    }

                    await oldFolder.DeleteAsync();

                    await client.DisconnectAsync(true);
                    MessageBox.Show("Папку перейменовано (створено нову, переміщено листи, стару видалено).");

                    ViewFolders();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка перейменування: " + ex.Message);
                }
            }
        }
        private void renameBoxEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (listFolder.SelectedItem != null && !string.IsNullOrWhiteSpace(renameBox.Text))
                {
                    string oldFolder = foldersName[listFolder.SelectedItem.ToString()];
                    string newFolder = renameBox.Text;
                    RenameFolder(oldFolder, newFolder);
                }
            }
        }

        private async void CreateFolder()
        {
            using (var client = new ImapClient())
            {
                try
                {
                    await client.ConnectAsync("imap.gmail.com", 993, MailKit.Security.SecureSocketOptions.SslOnConnect);
                    await client.AuthenticateAsync(username, password);

                    string newFolderName = createBox.Text;

                    var personalNs = client.PersonalNamespaces[0];
                    var parent = client.GetFolder(personalNs);
                    await parent.CreateAsync(newFolderName,true);

                    MessageBox.Show($"Папку {newFolderName} створено");

                    ViewFolders();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Папку не вдалосья створити");
                }
            }
        }
        private void createBoxEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CreateFolder();
            }
        }
        private void FilterMessage(object sender, RoutedEventArgs e)
        {
            listMessages.Items.Clear();

            foreach (var message in Messages)
            {
                if (message.Priority == MessagePriority.Urgent)
                {
                    listMessages.Items.Add($"{message.Date}: {message.Subject}");
                }
            }
        }
    }
}