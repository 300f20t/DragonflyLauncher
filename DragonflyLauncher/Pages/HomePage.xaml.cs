﻿using CmlLib.Core.Auth;
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DragonflyLauncher.Pages
{
    /// <summary>
    /// Логика взаимодействия для HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        string mineDir = @"%AppData%\.minecraft";
        string login = "Player";
        string selectedVersion = "1.20.1";

        public HomePage()
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
            await launcher.InstallAsync(selectedVersion);
            var process = await launcher.BuildProcessAsync(selectedVersion, new MLaunchOption
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
