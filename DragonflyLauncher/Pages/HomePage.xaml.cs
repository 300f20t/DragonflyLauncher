using CmlLib.Core.Auth;
using CmlLib.Core.ProcessBuilder;
using CmlLib.Core;
using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CmlLib.Core.Version;
using System.Text.Json;

namespace DragonflyLauncher.Pages
{
    /// <summary>
    /// Логика взаимодействия для HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public string Memory { get; set; }
        public string PlayerNickname { get; set; }

        string _minecraftDirectory = @"%AppData%\.minecraft";
        string _selectedVersion = "1.20.1";
        string _launcherConfigsString;

        public HomePage()
        {
            InitializeComponent();
            GetVersions();
        }


        private async void GetVersions()
        {

            var launcher = new MinecraftLauncher();
            try
            {
                var versions = await launcher.GetAllVersionsAsync();
                foreach (var version in versions)
                {
                    Console.WriteLine("Name: " + version.Name);
                    Console.WriteLine("Type: " + version.Type);
                    Console.WriteLine("ReleaseTime: " + version.ReleaseTime);

                    versionsComboBox.Items.Add(version.Name);
                }
            }
            catch { MessageBox.Show("No internet connection!"); }
        }

        private void PlayClick(object sender, RoutedEventArgs e)
        {
                RunMinecraft();
        }

        private async void RunMinecraft()
        {
            _launcherConfigsString = File.ReadAllText(@"LauncherConfigs.json");

            HomePage? launcherConfigs = JsonSerializer.Deserialize<HomePage>(_launcherConfigsString);

            MessageBox.Show(launcherConfigs.Memory.ToString());

            MinecraftLoadingInfo.Visibility = Visibility.Visible;

            if (versionsComboBox.Text != "") _selectedVersion = versionsComboBox.Text;

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
                MinecraftLoadingProgress.Value = args.ProgressedBytes;
            };

            // get all versions
            var versions = await launcher.GetAllVersionsAsync();
            foreach (var v in versions)
            {
                Console.WriteLine(v.Name);
            }

            // install and launch the game
            await launcher.InstallAsync(_selectedVersion);
            var process = await launcher.BuildProcessAsync(_selectedVersion, new MLaunchOption
            {
                Session = MSession.CreateOfflineSession(launcherConfigs.PlayerNickname),
                MaximumRamMb = Int32.Parse(launcherConfigs.Memory),
            });
            process.Start();

            MinecraftLoadingInfo.Visibility = Visibility.Hidden;

            MainWindow.GetWindow(this).Hide();

            while (true)
            {
                Thread.Sleep(10);
                if (process.HasExited)
                {
                    MainWindow.GetWindow(this).Show();
                    break;
                }
            }
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
            string absoluteMineDir = Environment.ExpandEnvironmentVariables(_minecraftDirectory);
            Process.Start("explorer.exe", @$"{absoluteMineDir}");
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            nickTextBox.Visibility = Visibility.Visible;
            accountsTextBlock.Text = "Login";
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SettingsPage());
        }
    }
}
