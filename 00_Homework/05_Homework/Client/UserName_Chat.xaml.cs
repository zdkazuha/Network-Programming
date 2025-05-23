﻿using System;
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

namespace Client
{
    /// <summary>
    /// Interaction logic for UserName_Chat.xaml
    /// </summary>
    public partial class UserName_Chat : Window
    {
        public string UserName;
        public UserName_Chat()
        {
            InitializeComponent();
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            string UserName = userName.Text;

            if (string.IsNullOrWhiteSpace(UserName))
            {
                MessageBox.Show("Будь ласка введіть свій нікнейм!");
                return;
            }
            else if (UserName.Length > 20 || UserName.Length < 4)
            {
                MessageBox.Show("Нікнейм не може бути більше 20 або меньше 4 символів!");
                return;
            }
            else if (UserName.Contains(":"))
            {
                MessageBox.Show("Нікнейм не може містити символ ':'");
                return;
            }

            MainWindow mainWindow = new MainWindow(UserName);
            this.Close();
            mainWindow.Show();
        }

        private void userName_(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                Login(sender, e);
        }
    }
}

