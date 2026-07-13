using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using GameVault.Models;
using GameVault.Converters;
using GameVault.Services;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Linq;

namespace GameVault
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<Game> Games { get; }

        private readonly GameStorageService storageService;

        private bool hasGames;

        public bool HasGames
        {
            get => hasGames;
            private set
            {
                hasGames = value;
                OnPropertyChanged(nameof(HasGames));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Games_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            HasGames = Games.Count > 0;
        }

        private async void ImportSteamButton_Click(object sender, RoutedEventArgs e)
        {

            var settingsService = new SettingsService();

            var settings = settingsService.LoadSettings();

            if (string.IsNullOrEmpty(settings.SteamId))
            {
                MessageBox.Show("Please set your Steam ID in the settings first.");
                return;
            }

            if (string.IsNullOrEmpty(settings.SteamApiKey))
            {
                MessageBox.Show("Please set your Steam API key in the settings first.");
                return;
            }

            var steamLibraryService = new SteamLibraryService(settings.SteamApiKey);

            var steamGames = await steamLibraryService.GetOwnedGames(settings.SteamId);

            int added = 0;
            int updated = 0;


            foreach (var steamGame in steamGames)
            {
                if (settings.IgnoredSteamAppIds.Contains(steamGame.AppId))
                {
                    continue;
                }

                var existingGame = Games.FirstOrDefault(g => g.SteamAppId == steamGame.AppId);

                SteamGameInfo? info = null;


                if (existingGame == null || string.IsNullOrWhiteSpace(existingGame.SteamGenres))
                {
                    info = await steamLibraryService.GetGameInfo( steamGame.AppId);

                    await Task.Delay(300); // Delay to avoid hitting Steam API rate limits
                }

                var achievements = await steamLibraryService.GetAchievements(settings.SteamId, steamGame.AppId);

                await Task.Delay(300); // Delay to avoid hitting Steam API rate limits


                if (existingGame != null)
                {
                    existingGame.PlaytimeHours = steamGame.PlaytimeHours;

                    if (info != null && info.Genres.Any())
                    {
                        existingGame.SteamGenres = string.Join(", ", info.Genres.Take(5));
                    }

                    if (achievements != null)
                    {
                        existingGame.AchievementsUnlocked = achievements.Value.Unlocked;
                        existingGame.AchievementsTotal = achievements.Value.Total;
                    }

                    updated++;
                }
                else
                {
                    var newGame = ConvertSteamGame(steamGame, info);

                    if (achievements != null)
                    {
                        newGame.AchievementsUnlocked = achievements.Value.Unlocked;
                        newGame.AchievementsTotal = achievements.Value.Total;
                    }

                    Games.Add(newGame);

                    added++;

                    gamesView.Refresh();
                }
            }



            storageService.SaveGames(Games);


            MessageBox.Show(
                $"Added: {added}\nUpdated: {updated}", "Steam Sync");
        }


        private void AddGameButton_Click(object sender, RoutedEventArgs e)
        {
            var addGameWindow = new Views.AddGameWindow();

            addGameWindow.Owner = this;

            if (addGameWindow.ShowDialog() == true && addGameWindow.CreatedGame is Game game)
            {
                Games.Add(game);

                storageService.SaveGames(Games);
            }
        }

        private void LoadGames()
        {
            var loadedGames = storageService.LoadGames();

            foreach (var game in loadedGames)
            {
                Games.Add(game);
            }
        }

        private Game ConvertSteamGame(SteamOwnedGame steamGame, SteamGameInfo? info)
        {
            return new Game
            {
                Name = steamGame.Name,
                SteamAppId = steamGame.AppId,
                PlaytimeHours = steamGame.PlaytimeHours,
                Status = GameStatus.NotStarted,
                SteamGenres = info != null && info.Genres.Any()
                    ? string.Join(", ", info.Genres.Take(5))
                    : "No genres"
            };
        }

        private Game? selectedGame;

        public Game? SelectedGame
        {
            get => selectedGame;
            set
            {
                selectedGame = value;

                OnPropertyChanged(nameof(SelectedGame));
                OnPropertyChanged(nameof(HasSelectedGame));
            }
        }

        public bool HasSelectedGame => SelectedGame != null;

        private void EditGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedGame == null)
            {
                MessageBox.Show("Please select a game first.");
                return;
            }
            
            var editWindow = new Views.AddGameWindow(SelectedGame);

            editWindow.Owner = this;

            if (editWindow.ShowDialog() == true)
            {
                storageService.SaveGames(Games);
            }
        }

        


        private void DeleteGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedGame == null)
            {
                MessageBox.Show("Please select a game first.");
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete {SelectedGame.Name}?",
                "Delete Game",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if (SelectedGame.SteamAppId is int steamAppId)
                {
                    var settingsService = new SettingsService();
                    var settings = settingsService.LoadSettings();

                    if (!settings.IgnoredSteamAppIds.Contains(steamAppId))
                    {
                        settings.IgnoredSteamAppIds.Add(steamAppId);
                        settingsService.SaveSettings(settings);
                    }
                }

                Games.Remove(SelectedGame);

                storageService.SaveGames(Games);
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new Views.SettingsWindow();

            window.Owner = this;
            
            window.ShowDialog();
        }

        private GameStatusFilter? selectedStatus;

        public GameStatusFilter? SelectedStatus
        {
            get => selectedStatus;
            set
            {
                selectedStatus = value;

                OnPropertyChanged(nameof(SelectedStatus));

                gamesView.Refresh();
            }
        }

        private ICollectionView gamesView;

        public ICollectionView GamesView => gamesView;

        public IEnumerable<GameStatusFilter> AvailableStatuses { get; }

        private string searchText = string.Empty;

        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                OnPropertyChanged(nameof(SearchText));

                gamesView.Refresh();
            }
        }

        private bool FilterGames(object obj)
        {
            if (obj is not Game game)
                return false;

            
            // Search podle názvu
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                if (!game.Name.Contains(
                    SearchText,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }


            // Filtr podle statusu
            if (SelectedStatus?.Status != null)
            {
                if (game.Status != SelectedStatus.Status.Value)
                {
                    return false;
                }
            }


            return true;
        }

        private List<GameStatusFilter> CreateStatusFilters()
        {
            return new List<GameStatusFilter>
    {
        new() { Name = "All", Status = null },
        new() { Name = "Not Started", Status = GameStatus.NotStarted },
        new() { Name = "Playing", Status = GameStatus.Playing },
        new() { Name = "Completed", Status = GameStatus.Completed },
        new() { Name = "Dropped", Status = GameStatus.Dropped }
    };
        }

        
        public MainWindow()
        {
            InitializeComponent();

            storageService = new GameStorageService();

            Games = new ObservableCollection<Game>();

            AvailableStatuses = CreateStatusFilters();

            gamesView = CollectionViewSource.GetDefaultView(Games);

            gamesView.Filter = FilterGames;

            SelectedStatus = AvailableStatuses.First();

            Games.CollectionChanged += Games_CollectionChanged;

            DataContext = this;

            LoadGames();
        }

    }
}