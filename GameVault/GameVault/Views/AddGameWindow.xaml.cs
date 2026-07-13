using GameVault.Models;
using GameVault.ViewModels;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace GameVault.Views
{

    public partial class AddGameWindow : Window
    {
        public event EventHandler<Game>? GameAdded;

        private readonly AddGameViewModel viewModel;

        public Game? CreatedGame { get; set; }
    
        public AddGameWindow(Game? game = null)
        {
            InitializeComponent();

            viewModel = new AddGameViewModel(game);

            viewModel.GameSaved += ViewModel_GameAdded;

            DataContext = viewModel;
        }

        private void ViewModel_GameAdded(object? sender, Game e)
        {
            CreatedGame = e;

            DialogResult = true;

        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select Game Executable",
                Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                viewModel.ExecutablePath = dialog.FileName;
            }
        }
    }
}
