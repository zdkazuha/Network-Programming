using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net;
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

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void GetFactorial_Btn(object sender, RoutedEventArgs e)
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ipBox.Text), int.Parse(portBox.Text));
            string message = txtBox.Text;

            TcpClient? client = null;

            try
            {
                client = new TcpClient();
                await client.ConnectAsync(ipPoint);
                NetworkStream stream = client.GetStream();

                StreamWriter sw = new StreamWriter(stream);
                await sw.WriteLineAsync(message);
                await sw.FlushAsync();

                StreamReader sr = new StreamReader(stream);
                string? result = await sr.ReadLineAsync();
                sr.Close();
                sw.Close();
                stream.Close();

                list.Items.Add(result);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                client?.Close();
            }
        }

        private void InputNumber(object sender, TextChangedEventArgs e)
        {
            if (!checkLineNumber(txtBox.Text))
            {
                txtBox.Background = Brushes.Red;
            }
            else
            {
                txtBox.Background = new SolidColorBrush(Colors.White);
            }
        }

        private bool checkLineNumber(string text)
        {
            return text.All((s) => char.IsDigit(s));
        }
    }
}