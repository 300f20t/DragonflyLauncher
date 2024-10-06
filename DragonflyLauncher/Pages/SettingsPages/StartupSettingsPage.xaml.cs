using System;
using System.Management;
using System.Windows;
using System.Windows.Controls;

namespace DragonflyLauncher.Pages.SettingsPages
{
    public partial class StartupSettingsPage : Page
    {
        public ulong TotalMemoryInMb { get; private set; }

        public StartupSettingsPage()
        {
            InitializeComponent();
            TotalMemoryInMb = GetTotalPhysicalMemoryInMb();
            // Устанавливаем максимальное значение слайдера
            MemorySlider.Maximum = TotalMemoryInMb;
            // Устанавливаем начальное значение
            MemorySlider.Value = 1024; // Например, 1 GB
            MemoryTextBox.Text = "1024"; // Начальное значение
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

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Проверка на null перед использованием
            if (MemoryTextBox != null)
            {
                MemoryTextBox.Text = ((int)e.NewValue).ToString();
            }
        }

        private void MemoryTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Обработка изменения текста в TextBox
            if (int.TryParse(MemoryTextBox.Text, out int newValue))
            {
                // Приведение TotalMemoryInMb к int и проверка
                if (newValue >= 0 && newValue <= (int)TotalMemoryInMb)
                {
                    MemorySlider.Value = newValue;
                }
                else
                {
                    MessageBox.Show($"Введите значение от 0 до {(int)TotalMemoryInMb} МБ.");
                }
            }
        }
    }
}
