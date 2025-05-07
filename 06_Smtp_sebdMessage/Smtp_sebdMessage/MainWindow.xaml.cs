using Microsoft.Win32;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
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

namespace Smtp_sebdMessage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    [AddINotifyPropertyChangedInterface]
    public partial class MainWindow : Window
    {

        const string server = "smtp.gmail.com";
        const short  port   =  587;

        string username;
        string password;

        public ObservableCollection<MessageList> Files { get; set; } = new ObservableCollection<MessageList>();
        public string mailPriority = "Normal";
        public MainWindow(string userName,string Password)
        {
            InitializeComponent();
            username = userName;
            password = Password;

            ComboBoxSelected.SelectionChanged += ComboBox_MailPriority;
            DataContext = this;
        }

        private void SelectFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
                Files.Add(new MessageList { FilePath = dialog.FileName });
        }
        private void SendMessage(object sender, RoutedEventArgs e)
        {
            MailMessage message = new MailMessage(username, toBox.Text, themeBox.Text,GetRichText(messageBox));

            if(mailPriority == "Normal")
                message.Priority = MailPriority.Normal;
            else if (mailPriority == "High")
                message.Priority = MailPriority.High;
            else
                message.Priority = MailPriority.Normal;
            
            foreach (var file in Files)
                message.Attachments.Add(new Attachment(file.FilePath));

            SmtpClient client = new SmtpClient(server, port);
            client.Credentials = new NetworkCredential(username, password);
            client.EnableSsl = true;

            client.SendAsync(message, message);

            client.SendCompleted += SebdMessageCompleted;
        }
        private void SebdMessageCompleted(object sender, AsyncCompletedEventArgs e)
        {
            MessageBox.Show("Повідомлення відправлено");
        }
        string GetRichText(RichTextBox richTextBox)
        {
            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd); 
            return textRange.Text;
        }
        private void ComboBox_MailPriority(object sender, SelectionChangedEventArgs e)
        {
            int index = ComboBoxSelected.SelectedIndex;
            switch(index)
            {
                case 0:
                    mailPriority = "Normal";
                    break;
                case 1:
                    mailPriority = "High";
                    break;
                default:
                    mailPriority = "Normal";
                    break;
            }
        }
        private void DeleteFile_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is MessageList item)
                Files.Remove(item);
        }

    }
    [AddINotifyPropertyChangedInterface]
    public class MessageList
    {
        public string FilePath { get; set; }
    }
}