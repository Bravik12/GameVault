using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GameVault.Models;
using GameVault.Services;

namespace GameVault.Views
{
    public partial class SettingsWindow : Window
    {
        private readonly SettingsService settingsService;


        public SettingsWindow()
        {
            InitializeComponent();

            settingsService = new SettingsService();


            var settings = settingsService.LoadSettings();

            SteamIdTextBox.Text = settings.SteamId;
            SteamApiKeyTextBox.Text = settings.SteamApiKey;
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(SteamIdTextBox.Text))
            {
                MessageBox.Show(
                    "Steam ID cannot be empty.",
                    "Settings");

                return;
            }

            var settings = settingsService.LoadSettings();

            settings.SteamId = SteamIdTextBox.Text;
            settings.SteamApiKey = SteamApiKeyTextBox.Text;

            settingsService.SaveSettings(settings);


            MessageBox.Show(
                "Settings saved.",
                "GameVault");


            Close();
        }
    }
}