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

        string _selectedVersion = "1.20.1";

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
            // Загружаем конфигурацию с помощью нового класса
            LauncherConfig? launcherConfigs = await LauncherConfig.LoadConfigurationAsync();
            if (launcherConfigs == null) return;

            // Установка видимости элемента интерфейса
            MinecraftLoadingInfo.Visibility = Visibility.Visible;

            // Проверка и установка выбранной версии Minecraft
            if (!string.IsNullOrEmpty(versionsComboBox.Text))
            {
                _selectedVersion = versionsComboBox.Text;
            }

            // Конфигурация максимального количества соединений
            System.Net.ServicePointManager.DefaultConnectionLimit = 256;

            // Инициализация лаунчера Minecraft
            var minecraftLauncher = new MinecraftLauncher(_minecraftPath);

            // Обработка прогресса загрузки файлов
            minecraftLauncher.FileProgressChanged += (sender, args) =>
            {
                Console.WriteLine($"File: {args.Name}, Type: {args.EventType}, Progress: {args.ProgressedTasks}/{args.TotalTasks}");
            };

            try
            {
                // Установка и запуск игры
                await minecraftLauncher.InstallAsync(_selectedVersion);
                var launchOptions = new MLaunchOption
                {
                    Session = MSession.CreateOfflineSession(launcherConfigs.PlayerNickname),
                    MaximumRamMb = int.Parse(launcherConfigs.Memory)
                };

                // Создание процесса Minecraft
                var process = await minecraftLauncher.BuildProcessAsync(_selectedVersion, launchOptions);
                process.Start();

                // Скрытие основного окна на время работы Minecraft
                MinecraftLoadingInfo.Visibility = Visibility.Hidden;
                var mainWindow = MainWindow.GetWindow(this);
                mainWindow.Hide();

                // Ожидание завершения процесса игры асинхронно
                await Task.Run(() => process.WaitForExit());

                // Возврат окна после закрытия Minecraft
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error launching Minecraft: {ex.Message}");
            }
            finally
            {
                // Скрытие индикатора загрузки в случае ошибки или завершения работы
                MinecraftLoadingInfo.Visibility = Visibility.Hidden;
            }
        }

        private async void ProgressLoading()
        {
            _minecraftLauncher.ByteProgressChanged += (sender, args) =>
            {
                Dispatcher.Invoke(() =>
                {
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
            nickTextBox.Visibility = Visibility.Hidden;
            accountsTextBlock.Text = "Login with";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/") { UseShellExecute = true });
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
