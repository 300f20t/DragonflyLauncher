using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.ProcessBuilder;
using CmlLib.Core.Version;
using DragonflyLauncher.Configurations;

namespace DragonflyLauncher.Pages
{
    public partial class HomePage : Page
    {
        static string _minecraftDirectory = @"%AppData%\.minecraft";
        static string _absoluteMinecraftDirectory = Environment.ExpandEnvironmentVariables(_minecraftDirectory);

        string _selectedVersion = "1.20.1";  // Значение по умолчанию

        MinecraftPath _minecraftPath = new(_absoluteMinecraftDirectory);
        MinecraftLauncher _minecraftLauncher = new();

        public HomePage()
        {
            InitializeComponent();
            GetVersions();
        }

        private async void GetVersions()
        {
            try
            {
                var versions = await _minecraftLauncher.GetAllVersionsAsync();
                foreach (var version in versions)
                {
                    versionsComboBox.Items.Add(version.Name);
                }
                versionsComboBox.SelectedIndex = 0;
            }
            catch
            {
                MessageBox.Show("No internet connection!");
            }
        }

        private void PlayClick(object sender, RoutedEventArgs e)
        {
            RunMinecraft();
        }

        private async void RunMinecraft()
        {
            LauncherConfig? launcherConfigs = await LoadAndValidateConfigurationsAsync();
            if (launcherConfigs == null) return;

            // Установка видимости элемента интерфейса
            ToggleLoadingIndicator(true);

            // Установка выбранной версии, если пользователь выбрал из ComboBox
            if (versionsComboBox.SelectedItem != null)
            {
                _selectedVersion = versionsComboBox.SelectedItem.ToString();
            }

            try
            {
                // Вызов метода для отслеживания прогресса загрузки перед установкой
                ProgressLoading();

                await LaunchMinecraft(launcherConfigs);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error launching Minecraft: {ex.Message}");
            }
            finally
            {
                ToggleLoadingIndicator(false);
            }
        }

        private async Task<LauncherConfig?> LoadAndValidateConfigurationsAsync()
        {
            LauncherConfig? launcherConfigs = await LauncherConfig.LoadConfigurationAsync();
            if (launcherConfigs == null || string.IsNullOrEmpty(launcherConfigs.PlayerNickname) || string.IsNullOrEmpty(launcherConfigs.Memory))
            {
                MessageBox.Show("Configuration is missing or incomplete.");
                return null;
            }
            return launcherConfigs;
        }

        private async Task LaunchMinecraft(LauncherConfig launcherConfigs)
        {
            // Установка и запуск игры
            await _minecraftLauncher.InstallAsync(_selectedVersion);
            var launchOptions = new MLaunchOption
            {
                Session = MSession.CreateOfflineSession(launcherConfigs.PlayerNickname),
                MaximumRamMb = int.Parse(launcherConfigs.Memory)
            };

            // Создание процесса Minecraft
            var process = await _minecraftLauncher.BuildProcessAsync(_selectedVersion, launchOptions);
            process.Start();

            var mainWindow = MainWindow.GetWindow(this);
            mainWindow.Hide();
            await Task.Run(() => process.WaitForExit());
            mainWindow.Show();
        }

        private void ToggleLoadingIndicator(bool isLoading)
        {
            MinecraftLoadingInfo.Visibility = isLoading ? Visibility.Visible : Visibility.Hidden;
        }

        private void ProgressLoading()
        {
            _minecraftLauncher.ByteProgressChanged += (sender, args) =>
            {
                Dispatcher.Invoke(() =>
                {
                    // Обновление значения ProgressBar
                    MinecraftLoadingProgress.Value = (double)args.ProgressedBytes / args.TotalBytes * 100;
                });
            };
        }

        private void ModsButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("WIP");
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/300f20t/DragonflyLauncher") { UseShellExecute = true });
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", @$"{_absoluteMinecraftDirectory}");
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
