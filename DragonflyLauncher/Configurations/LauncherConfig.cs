using System;
using System.IO;
using System.Windows;
using System.Text.Json;
using System.Threading.Tasks;

namespace DragonflyLauncher.Configurations
{
    public class LauncherConfig
    {
        public string Memory { get; set; }
        public string PlayerNickname { get; set; }

        private static string _configFile = @"Configurations\LauncherConfigs.json";

        /// <summary>
        /// Асинхронно загружает конфигурацию из файла.
        /// </summary>
        public static async Task<LauncherConfig?> LoadConfigurationAsync()
        {
            try
            {
                if (!File.Exists(_configFile))
                {
                    MessageBox.Show($"Configuration file '{_configFile}' is missing!");
                    return null;
                }

                string configData = await File.ReadAllTextAsync(_configFile);
                var launcherConfig = JsonSerializer.Deserialize<LauncherConfig>(configData);

                if (launcherConfig == null || string.IsNullOrEmpty(launcherConfig.PlayerNickname) || string.IsNullOrEmpty(launcherConfig.Memory))
                {
                    MessageBox.Show("Failed to read the configuration file or configuration fields are empty. Check the file format.");
                    return null;
                }

                return launcherConfig;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading configuration file: {ex.Message}");
                return null;
            }
        }
    }
}
