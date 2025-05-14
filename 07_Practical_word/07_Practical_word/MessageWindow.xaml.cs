using Microsoft.Win32;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace _07_Practical_word
{
    /// <summary>
    /// Interaction logic for MessageWindow.xaml
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public partial class MessageWindow : Window
    {

        const string server = "smtp.gmail.com";
        const short port = 587;

        string username;
        string password;

        public ObservableCollection<MessageList> Files { get; set; } = new ObservableCollection<MessageList>();
        public string mailPriority = "Normal";

        public MimeKit.MimeMessage mail_;

        public MessageWindow(string userName, string Password, MimeKit.MimeMessage mail)
        {
            InitializeComponent();

            username = userName;
            password = Password;

            mail_ = mail;

            fromBox.Text = mail.From.Mailboxes.FirstOrDefault().Address;
            toBox.Text = mail.To.ToString();
            themeBox.Text = mail.Subject.ToString();
            dateBox.Text = mail.Date.Date.ToShortDateString().ToString();
            if(mail.TextBody == null || string.IsNullOrWhiteSpace(mail.TextBody.ToString()))
                messageBox.AppendText("");
            else
                messageBox.AppendText(mail.TextBody.ToString());
            
            foreach (var item in mail.Attachments)
                Files.Add(new MessageList { FilePath = item.ContentDisposition.FileName });
            
            ComboBoxSelected.SelectionChanged += ComboBox_MailPriority;
            DataContext = this;
        }

        public MessageWindow(string userName, string Password)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

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
        private bool Audit()
        {
            if (
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(toBox.Text) ||
                string.IsNullOrWhiteSpace(themeBox.Text) ||
                string.IsNullOrWhiteSpace(GetRichText(messageBox))
            )
            {
                MessageBox.Show("Будь ласка, заповніть всі обов’язкові поля (Кому, Тема, Повідомлення).");
                return false;
            }

            return true;
        }
        private void SendMessage(object sender, RoutedEventArgs e)
        {
            if(!Audit())
                return;

            MailMessage message = new MailMessage(username, toBox.Text, themeBox.Text, GetRichText(messageBox));

            if (mailPriority == "Normal")
                message.Priority = MailPriority.Normal;
            else if (mailPriority == "High")
                message.Priority = MailPriority.High;
            else
                message.Priority = MailPriority.Normal;

            foreach (var file in Files)
            {
                try
                {
                    message.Attachments.Add(new Attachment(file.FilePath));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Повідомлення було вдправленоо без файла");
                }
            }

            SmtpClient client = new SmtpClient(server, port);
            client.Credentials = new NetworkCredential(username, password);
            client.EnableSsl = true;

            client.SendCompleted += SendMessageCompleted;

            client.SendAsync(message, message);

            dateBox.Text = DateTime.Now.ToShortDateString();
        }
        private void AnswerMessage(object sender, RoutedEventArgs e)
        {
            if (!Audit())
                return;

            MailMessage message = new MailMessage(username, toBox.Text, "Відповідь" , GetRichText(messageBox));

            if (mailPriority == "Normal")
                message.Priority = MailPriority.Normal;
            else if (mailPriority == "High")
                message.Priority = MailPriority.High;
            else
                message.Priority = MailPriority.Normal;

            foreach (var file in Files)
            {
                try
                {
                    message.Attachments.Add(new Attachment(file.FilePath));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Повідомлення було вдправленоо без файла");
                }
            }

            SmtpClient client = new SmtpClient(server, port);
            client.Credentials = new NetworkCredential(username, password);
            client.EnableSsl = true;

            client.SendAsync(message, message);

            client.SendCompleted += SendMessageCompleted;

            dateBox.Text = DateTime.Now.ToShortDateString();
        }
        string GetRichText(RichTextBox richTextBox)
        {
            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            return textRange.Text;
        }
        private void ComboBox_MailPriority(object sender, SelectionChangedEventArgs e)
        {
            int index = ComboBoxSelected.SelectedIndex;
            switch (index)
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
        private void SendMessageCompleted(object sender, AsyncCompletedEventArgs e)
        {
            MessageBox.Show("Повідомлення відправлено");
        }
        private void Clear(object sender, RoutedEventArgs e)
        {
            fromBox.Text = "";
            toBox.Text = "";
            themeBox.Text = "";
            dateBox.Text = DateTime.Now.ToShortDateString();
            messageBox.Document.Blocks.Clear();

            Files.Clear();
        }
    }
    [AddINotifyPropertyChangedInterface]
    public class MessageList
    {
        public string FilePath { get; set; }
    }
}