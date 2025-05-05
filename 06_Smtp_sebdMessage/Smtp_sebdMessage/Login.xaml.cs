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

            MainWindow mainWindow = new MainWindow(UserName.Text, Password.Text);
            mainWindow.Show();
            this.Close();
        }
    }
}
