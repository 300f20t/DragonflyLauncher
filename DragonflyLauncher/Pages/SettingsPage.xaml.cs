using DragonflyLauncher.Pages.SettingsPages;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using DragonflyLauncher.Configurations;

namespace DragonflyLauncher.Pages
{
    public partial class SettingsPage : Page
    {
        private StartupSettingsPage _startupSettingsPage;

        public SettingsPage()
        {
            InitializeComponent();
            _startupSettingsPage = new StartupSettingsPage();
            SettingsFrame.Content = _startupSettingsPage;
        }

        private async void SaveButton(object sender, RoutedEventArgs e)
        {
            if (_startupSettingsPage != null)
            {
                int memoryValue = _startupSettingsPage.SelectedMemoryValue;

                // Загрузка текущей конфигурации
                var config = await LauncherConfig.LoadConfigurationAsync() ?? new LauncherConfig();
                config.Memory = memoryValue.ToString();

                // Сохранение обновленной конфигурации
                await LauncherConfig.SaveConfigurationAsync(config);

                MessageBox.Show("Настройки сохранены.");
            }
        }

        private void BackButton(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HomePage());
        }

        private void StartupSettingsButton(object sender, RoutedEventArgs e)
        {
            _startupSettingsPage = new StartupSettingsPage();
            SettingsFrame.Content = _startupSettingsPage;
        }

        private void LauncherSettingsButton(object sender, RoutedEventArgs e)
        {
            SettingsFrame.Content = new LauncherSettingsPage();
        }

        private void ModBuilderSettingsButton(object sender, RoutedEventArgs e)
        {
            SettingsFrame.Content = new ModBuilderSettingsPage();
        }
    }
}
