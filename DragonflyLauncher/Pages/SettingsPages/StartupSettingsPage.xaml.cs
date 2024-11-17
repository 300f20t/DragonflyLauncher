using System;
using System.Management;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using DragonflyLauncher.Configurations;

namespace DragonflyLauncher.Pages.SettingsPages
{
    public partial class StartupSettingsPage : Page
    {
        public ulong TotalMemoryInMb { get; private set; }
        private LauncherConfig _config;

        public int SelectedMemoryValue { get; private set; }

        public StartupSettingsPage()
        {
            InitializeComponent();
            TotalMemoryInMb = GetTotalPhysicalMemoryInMb();
            MemorySlider.Maximum = TotalMemoryInMb;
            MemorySlider.Value = 1024;
            MemoryTextBox.Text = "1024";
            LoadConfigurationAsync();
        }

        private ulong GetTotalPhysicalMemoryInMb()
        {
            ulong totalMemoryMb = 0;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory");

            foreach (ManagementObject obj in searcher.Get())
            {
                totalMemoryMb += Convert.ToUInt64(obj["Capacity"]) / (1024 * 1024);
            }

            return totalMemoryMb;
        }

        private async Task LoadConfigurationAsync()
        {
            _config = await LauncherConfig.LoadConfigurationAsync() ?? new LauncherConfig();
            if (int.TryParse(_config.Memory, out int memoryValue) && memoryValue <= (int)TotalMemoryInMb)
            {
                MemorySlider.Value = memoryValue;
                MemoryTextBox.Text = memoryValue.ToString();
                SelectedMemoryValue = memoryValue;
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MemoryTextBox != null)
            {
                int memoryValue = (int)e.NewValue;
                MemoryTextBox.Text = memoryValue.ToString();
                SelectedMemoryValue = memoryValue;
            }
        }

        private void MemoryTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(MemoryTextBox.Text, out int newValue))
            {
                if (newValue >= 0 && newValue <= (int)TotalMemoryInMb)
                {
                    MemorySlider.Value = newValue;
                    SelectedMemoryValue = newValue;
                }
                else
                {
                    MessageBox.Show($"Enter a value from 0 to {(int)TotalMemoryInMb} MB.");
                }
            }
        }
    }
}
