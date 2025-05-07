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

namespace _07_Practical_word
{
    /// <summary>
    /// Interaction logic for MessageInfo.xaml
    /// </summary>
    public partial class MessageInfo : Window
    {
        public MessageInfo(string From,string To,string Subject)
        {
            InitializeComponent();
            FromBox.Text = From;
            ToBox.Text = To;
            ListSubject.Items.Add(Subject);
        }
    }
}
