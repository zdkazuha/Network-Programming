using Library;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
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
        private string example = "";
        
        TcpClient client;
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"),8080);
        
        NetworkStream ns;
        
        StreamWriter sw;
        StreamReader sr;
        
        public MainWindow()
        {
            InitializeComponent();
            client = new TcpClient();
            try
            {
                client.Connect(endPoint);
                ns = client.GetStream();
                sw = new StreamWriter(ns);
                sr = new StreamReader(ns);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Listener();
        }

        private void GetValueBtn(object sender, RoutedEventArgs e)
        {
            if((sender as Button)?.Content.ToString() != "=")
            {
                example += (sender as Button)?.Content.ToString();
                txtBox.Text = example;
            }
            else
            {
                Reg task = new Reg();

                var numbers = example.Split(new char[] { '+','-','/','*'},2);

                task.One = int.Parse(numbers[0]);
                task.Two = int.Parse(numbers[1]);
                task.Operation = getOperation(example);

                //MessageBox.Show($"{task.One} {task.Operation} {task.Two}");
                SendObj(task);
            }
        }
        private string getOperation(string example)
        {
            foreach (char item in example)
            {
                if (!Char.IsDigit(item))
                    return item.ToString();
            }
            return null!;
        }
        private void SendObj(Reg reg)
        {
            try
            {
                if (reg == null)
                    return;
                
                sw.WriteLine(JsonSerializer.Serialize(reg));
                sw.Flush();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SendObj()
        {
            try
            {
                sw.WriteLine("$<close>");
                sw.Flush();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            SendObj();
        }
        private async void Listener()
        {
            while (true)
            {
                try
                {
                    string? message = await sr.ReadLineAsync();
                    txtBox.Text = message;
                    example = "";
                }
                catch (Exception ex)
                {
                    break;
                }
            }
        }
    }
}