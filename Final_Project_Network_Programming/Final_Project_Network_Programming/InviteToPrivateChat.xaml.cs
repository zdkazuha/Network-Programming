using Db_Controller;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Final_Project_Network_Programming
{
    public partial class InviteToPrivateChat : Window
    {
        private Db_functional context = new Db_functional();
        private StreamWriter sw;
        private string UserName;

        public InviteToPrivateChat(StreamWriter sw_, string username)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            UserName = username;
            sw = sw_;
        }

        private async void InviteBtn(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(usernameBox.Text) || string.IsNullOrWhiteSpace(chatNameBox.Text))
            {
                MessageBox.Show("Будь ласка, введіть ім’я користувача або назву чату.");
                return;
            }

            string username = usernameBox.Text;
            string chatName = chatNameBox.Text;

            var user = context.GetUser(username);
            if (user != null)
            {
                if (sw == null)
                {
                    MessageBox.Show("Не вдалося відправити повідомлення: немає активного з'єднання.");
                    return;
                }

                try
                {
                    string message = $"{UserName}:Invite:{chatName}:{username}";
                    await sw.WriteLineAsync(message);
                    await sw.FlushAsync();

                    MessageBox.Show("Запрошення відправлено!");
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Сталася помилка при відправці запрошення: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Користувача з таким іменем не існує");
            }
        }

    }
}
