using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using GameVault.Models;
using GameVault.Services;

namespace GameVault.Views
{
    public partial class GameDetailPanel : UserControl
    {
        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register(
                nameof(IsEditing),
                typeof(bool),
                typeof(GameDetailPanel),
                new PropertyMetadata(false));

        public bool IsEditing
        {
            get => (bool)GetValue(IsEditingProperty);
            set => SetValue(IsEditingProperty, value);
        }

        public event EventHandler? CloseRequested;
        public event EventHandler<Game>? DeleteRequested;

        private Game? game;

        private string editSnapshotName = string.Empty;
        private GameStatus editSnapshotStatus;
        private double? editSnapshotRating;
        private string? editSnapshotNotes;

        public GameDetailPanel()
        {
            InitializeComponent();

            StatusComboBox.ItemsSource = Enum.GetValues<GameStatus>();
        }

        public void LoadGame(Game newGame)
        {
            if (IsEditing)
            {
                RevertToSnapshot();
            }

            game = newGame;

            DataContext = game;

            IsEditing = false;

            if (game.SteamAppId is int appId)
            {
                var isInstalled = new SteamInstallationService().IsGameInstalled(appId);

                PlayButton.Content = isInstalled ? "▶ Play" : "⬇ Install";
            }
            else
            {
                PlayButton.Content = "▶ Play";
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (game == null)
            {
                return;
            }

            editSnapshotName = game.Name;
            editSnapshotStatus = game.Status;
            editSnapshotRating = game.Rating;
            editSnapshotNotes = game.Notes;

            IsEditing = true;
        }

        private void CancelEditButton_Click(object sender, RoutedEventArgs e)
        {
            RevertToSnapshot();

            IsEditing = false;
        }

        private void RevertToSnapshot()
        {
            if (game == null)
            {
                return;
            }

            game.Name = editSnapshotName;
            game.Status = editSnapshotStatus;
            game.Rating = editSnapshotRating;
            game.Notes = editSnapshotNotes;
        }

        private void SaveEditButton_Click(object sender, RoutedEventArgs e)
        {
            IsEditing = false;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (game == null)
            {
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete {game.Name}?",
                "Delete Game",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                DeleteRequested?.Invoke(this, game);
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (game == null)
            {
                return;
            }

            try
            {
                if (game.SteamAppId is int appId)
                {
                    Process.Start(new ProcessStartInfo($"steam://run/{appId}")
                    {
                        UseShellExecute = true
                    });
                }
                else if (!string.IsNullOrWhiteSpace(game.ExecutablePath))
                {
                    Process.Start(new ProcessStartInfo(game.ExecutablePath)
                    {
                        UseShellExecute = true,
                        WorkingDirectory = Path.GetDirectoryName(game.ExecutablePath)
                    });
                }
                else
                {
                    MessageBox.Show("No launch path configured for this game.", "GameVault");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not launch the game: {ex.Message}", "GameVault");
            }
        }
    }
}
