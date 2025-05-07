using _07_Practical_word;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Smtp_sebdMessage
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        string username;
        string password;
        public Login()
        {
            InitializeComponent();
            UserName.Text = "aartemchelyk@gmail.com";
            Password.Text = "zlsu ftiw ttlr mjoi ";
        }

        private void LoginBtn(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(UserName.Text) || string.IsNullOrEmpty(Password.Text))
            {
                MessageBox.Show("Введіть емаіл або пароль");
                return;
            }
            username = UserName.Text;
            password = Password.Text;

            MainWindow mainWindow = new MainWindow(username, password);
            mainWindow.Show();
            this.Close();
        }
    }
}
