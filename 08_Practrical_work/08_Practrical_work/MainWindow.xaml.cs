using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WindowsAPICodePack.Dialogs;
using PropertyChanged;
using System.Diagnostics;

namespace _08_Practrical_work
{
    [AddINotifyPropertyChangedInterface]
    public partial class MainWindow : Window
    {
        private static string FolderPath = "";
        private int ID = 1;
        private object idLock = new object();

        public ObservableCollection<FileInfo> Files { get; set; } = new ObservableCollection<FileInfo>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Browse(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                FolderPath = dialog.FileName;
        }
        private void Download(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UrlFile.Text))
            {
                MessageBox.Show("Будь ласка, введіть URL файлу");
                return;
            }

            if (string.IsNullOrWhiteSpace(FolderPath))
            {
                MessageBox.Show("Будь ласка, виберіть папку для збереження");
                return;
            }

            string url = UrlFile.Text;
            Task.Run(() => DownloadFileAsync(url));
        }

        private async Task DownloadFileAsync(string url)
        {
            int currentId;
            lock (idLock)
            {
                currentId = ID++;
            }

            string fileName = $"image({currentId}).jpg";
            string outputPath = Path.Combine(FolderPath, fileName);

            var file = new FileInfo
            {
                FileName = fileName,
                TokenSource = new CancellationTokenSource(),
                Completed = "Downloading"
            };

            Application.Current.Dispatcher.Invoke(() => Files.Add(file));

            try
            {
                using var httpClient = new HttpClient();
                using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, file.TokenSource.Token);
                response.EnsureSuccessStatusCode();

                long totalBytes = response.Content.Headers.ContentLength ?? -1;
                file.SizeFile = totalBytes > 0 ? $"{totalBytes / 1024.0:F2} KB" : "Unknown";

                using var input = await response.Content.ReadAsStreamAsync(file.TokenSource.Token);
                using var output = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);

                byte[] buffer = new byte[1024];
                long totalRead = 0;
                int bytesRead;

                while ((bytesRead = await input.ReadAsync(buffer, 0, buffer.Length, file.TokenSource.Token)) > 0)
                {
                    await output.WriteAsync(buffer, 0, bytesRead, file.TokenSource.Token);
                    totalRead += bytesRead;

                    if (totalBytes > 0)
                    {
                        file.Percent = (float)(totalRead * 100.0 / totalBytes);
                    }

                    await Task.Delay(10);

                    file.Completed = "Downloading...";
                }

                file.Completed = "Completed";
            }
            catch (OperationCanceledException)
            {
                file.Completed = "Canceled";
            }
            catch (Exception ex)
            {
                file.Completed = $"Error: {ex.Message}";
            }
        }


        private void StopDownload(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is FileInfo file)
            {
                file.TokenSource?.Cancel();
            }
        }


        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var label = sender as Label;
            if (label?.DataContext is FileInfo file)
            {
                string path = Path.Combine(FolderPath, file.FileName);
                if (File.Exists(path))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = true
                    });
                }
            }
        }
    }

    [AddINotifyPropertyChangedInterface]
    public class FileInfo
    {
        public string FileName { get; set; }
        public float Percent { get; set; }
        public string Completed { get; set; }
        public string SizeFile { get; set; }
        public string PercentText => $"{Percent:F2}%";
        public CancellationTokenSource TokenSource { get; set; }
    }
}