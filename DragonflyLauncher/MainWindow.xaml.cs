using CmlLib.Core.Auth;
using CmlLib.Core.ProcessBuilder;
using CmlLib.Core;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Interop;
using DragonflyLauncher.Pages;

namespace DragonflyLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Content = new HomePage();
        }
    }
}