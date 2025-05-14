using Db_Controller;
using Db_Controller.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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

namespace Final_Project_Network_Programming
{
    /// <summary>
    /// Interaction logic for Confirmation.xaml
    /// </summary>
    public partial class ConfirmationGroup : Window
    {
        private User User;
        private string ChatName;

        public static Db_functional context = new Db_functional();

        public ConfirmationGroup(User user, string groupName)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            GropName.Content = groupName;
            UserName.Content = user.Username;

            User = user;
            ChatName = groupName;
        }

        private void YesBtn(object sender, RoutedEventArgs e)
        {
            context.AddUserToGroup(User, ChatName);
            MessageBox.Show($"Вітаємо! Ви приєдналися до групи {ChatName}.");
            this.Close();
        }

        private void NoBtn(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ви відмовились від запрошення.");
            this.Close();
        }
    }

}
