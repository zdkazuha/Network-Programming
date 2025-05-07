using Microsoft.Win32;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
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
using WindowsAPICodePack.Dialogs;
using static System.Net.WebRequestMethods;

namespace _08_Practrical_work
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    [AddINotifyPropertyChangedInterface]
    public partial class MainWindow : Window
    {
        WebClient webClient;
        HttpClient Client;
        string FolderPath;
        static int ID;

        public ObservableCollection<FileInfo> Files { get; set; } = new ObservableCollection<FileInfo>();
        public MainWindow()
        {
            InitializeComponent();
            webClient = new WebClient();
            Client = new HttpClient();
            FolderPath = "";
            UrlFile.Text = "";
            ID = 1;

            DataContext = this;
        }

        private void Browse(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                FolderPath = dialog.FileName;
        }
        private async void Download(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(UrlFile.Text))
            {
                MessageBox.Show("Будь ласка ведіть url файлу");
                return;
            }

            if (FolderPath == "" || FolderPath == null)
            {
                return;
            }

            byte[] data = await Client.GetByteArrayAsync(UrlFile.Text);
            System.IO.File.WriteAllBytes($@"{FolderPath}/image({ID}).jpg", data);
            Console.WriteLine("Completed loaded");

            Files.Add(new FileInfo()
            {
                FileName = $"image({ID}).jpg",
                Percant = "0%",
                Completed = "Completed"
            });
     
            ID++;
        }
    }

    [AddINotifyPropertyChangedInterface]
    public class FileInfo
    {
        public string FileName { get; set; }
        public string Percant { get; set; }
        public string Completed { get; set; } 
    }
}