﻿using DragonflyLauncher.Pages.SettingsPages;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            SettingsFrame.Content = new StartupSettingsPage();
        }

        private void SaveButton(object sender, RoutedEventArgs e)
        {

        }
        private void BackButton(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HomePage());
        }

        private void StartupSettingsButton(object sender, RoutedEventArgs e)
        {
            SettingsFrame.Content = new StartupSettingsPage();
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
