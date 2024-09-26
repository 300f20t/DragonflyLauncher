using CmlLib.Core.Auth;
using CmlLib.Core.ProcessBuilder;
using CmlLib.Core;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Interop;

namespace DragonflyLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string mineDir = @"%AppData%\.minecraft";
        string login = "Player";
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void PlayClick(object sender, RoutedEventArgs e)
        {
            if (nickTextBox.Text != "") login = nickTextBox.Text;

            System.Net.ServicePointManager.DefaultConnectionLimit = 256;

            // initialize the launcher
            var path = new MinecraftPath();
            var launcher = new MinecraftLauncher(path);

            // add event handlers
            launcher.FileProgressChanged += (sender, args) =>
            {
                Console.WriteLine($"Name: {args.Name}");
                Console.WriteLine($"Type: {args.EventType}");
                Console.WriteLine($"Total: {args.TotalTasks}");
                Console.WriteLine($"Progressed: {args.ProgressedTasks}");
            };
            launcher.ByteProgressChanged += (sender, args) =>
            {
                Console.WriteLine($"{args.ProgressedBytes} bytes / {args.TotalBytes} bytes");
            };

            // get all versions
            var versions = await launcher.GetAllVersionsAsync();
            foreach (var v in versions)
            {
                Console.WriteLine(v.Name);
            }

            // install and launch the game
            await launcher.InstallAsync("1.20.6");
            var process = await launcher.BuildProcessAsync("1.20.6", new MLaunchOption
            {
                Session = MSession.CreateOfflineSession(login),
                MaximumRamMb = 4096
            });
            process.Start();
        }

        private void ModsButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("WIP");
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            nickTextBox.Visibility = Visibility.Hidden;
            accountsTextBlock.Text = "Login with";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/") { UseShellExecute = true });
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string absoluteMineDir = Environment.ExpandEnvironmentVariables(mineDir);
            Process.Start("explorer.exe", @$"{absoluteMineDir}");
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            nickTextBox.Visibility = Visibility.Visible;
            accountsTextBlock.Text = "Login";
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("WIP");
        }
    }
}