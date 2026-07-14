using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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

        private readonly SteamInstallationService steamInstallationService = new();

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


                if (existingGame == null ||
                    string.IsNullOrWhiteSpace(existingGame.SteamGenres) ||
                    string.IsNullOrWhiteSpace(existingGame.Developer))
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

                    if (info != null)
                    {
                        existingGame.SteamImageUrl = info.HeaderImage;
                        existingGame.SteamDescription = info.ShortDescription;
                        existingGame.Developer = info.Developer;
                        existingGame.Publisher = info.Publisher;
                        existingGame.ReleaseDate = info.ReleaseDate;
                    }

                    if (achievements != null)
                    {
                        existingGame.AchievementsUnlocked = achievements.Value.Unlocked;
                        existingGame.AchievementsTotal = achievements.Value.Total;
                    }

                    RefreshInstallState(existingGame);

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

                    RefreshInstallState(newGame);

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
                RefreshInstallState(game);

                Games.Add(game);
            }
        }

        private void RefreshInstallState(Game game)
        {
            if (game.SteamAppId is int appId)
            {
                game.NeedsInstall = !steamInstallationService.IsGameInstalled(appId);
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
                IsFromSteam = true,
                SteamImageUrl = info?.HeaderImage,
                SteamDescription = info?.ShortDescription,
                Developer = info?.Developer,
                Publisher = info?.Publisher,
                ReleaseDate = info?.ReleaseDate,
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



        private void DetailPanel_DeleteRequested(object? sender, Game gameToDelete)
        {
            if (gameToDelete.SteamAppId is int steamAppId)
            {
                var settingsService = new SettingsService();
                var settings = settingsService.LoadSettings();

                if (!settings.IgnoredSteamAppIds.Contains(steamAppId))
                {
                    settings.IgnoredSteamAppIds.Add(steamAppId);
                    settingsService.SaveSettings(settings);
                }
            }

            Games.Remove(gameToDelete);

            storageService.SaveGames(Games);

            ClosePanel();
        }

        private void GamesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedGame == null)
            {
                return;
            }

            DetailPanel.LoadGame(SelectedGame);

            OpenPanel();
        }

        private void DetailOverlay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            storageService.SaveGames(Games);

            ClosePanel();
        }

        private void OpenPanel()
        {
            DetailOverlay.Visibility = Visibility.Visible;

            var easing = new CubicEase { EasingMode = EasingMode.EaseOut };

            DetailPanelTransform.BeginAnimation(
                TranslateTransform.XProperty,
                new DoubleAnimation(0, TimeSpan.FromMilliseconds(220)) { EasingFunction = easing });

            DetailOverlay.BeginAnimation(
                OpacityProperty,
                new DoubleAnimation(0.55, TimeSpan.FromMilliseconds(220)));
        }

        private void ClosePanel()
        {
            var easing = new CubicEase { EasingMode = EasingMode.EaseIn };

            var slideOut = new DoubleAnimation(DetailPanelHost.Width, TimeSpan.FromMilliseconds(200))
            {
                EasingFunction = easing
            };

            slideOut.Completed += (_, _) => DetailOverlay.Visibility = Visibility.Collapsed;

            DetailPanelTransform.BeginAnimation(TranslateTransform.XProperty, slideOut);

            DetailOverlay.BeginAnimation(
                OpacityProperty,
                new DoubleAnimation(0, TimeSpan.FromMilliseconds(200)));

            SelectedGame = null;
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

        private GameSortOption? selectedSortOption;

        public GameSortOption? SelectedSortOption
        {
            get => selectedSortOption;
            set
            {
                selectedSortOption = value;

                OnPropertyChanged(nameof(SelectedSortOption));

                gamesView.SortDescriptions.Clear();

                if (selectedSortOption?.PropertyName != null)
                {
                    gamesView.SortDescriptions.Add(
                        new SortDescription(selectedSortOption.PropertyName, selectedSortOption.Direction));
                }
            }
        }

        public IEnumerable<GameSortOption> AvailableSortOptions { get; }

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

        private List<GameSortOption> CreateSortOptions()
        {
            return new List<GameSortOption>
            {
                new() { Name = "Name (A-Z)", PropertyName = nameof(Game.Name), Direction = ListSortDirection.Ascending },
                new() { Name = "Rating (High-Low)", PropertyName = nameof(Game.Rating), Direction = ListSortDirection.Descending },
                new() { Name = "Playtime (High-Low)", PropertyName = nameof(Game.PlaytimeHours), Direction = ListSortDirection.Descending },
                new() { Name = "Achievements (High-Low)", PropertyName = nameof(Game.AchievementsPercent), Direction = ListSortDirection.Descending }
            };
        }


        public MainWindow()
        {
            InitializeComponent();

            storageService = new GameStorageService();

            Games = new ObservableCollection<Game>();

            AvailableStatuses = CreateStatusFilters();

            AvailableSortOptions = CreateSortOptions();

            gamesView = CollectionViewSource.GetDefaultView(Games);

            gamesView.Filter = FilterGames;

            SelectedStatus = AvailableStatuses.First();

            SelectedSortOption = AvailableSortOptions.First();

            Games.CollectionChanged += Games_CollectionChanged;

            DetailPanel.CloseRequested += DetailPanel_CloseRequested;
            DetailPanel.DeleteRequested += DetailPanel_DeleteRequested;

            DataContext = this;

            LoadGames();
        }

        private void DetailPanel_CloseRequested(object? sender, EventArgs e)
        {
            storageService.SaveGames(Games);

            ClosePanel();
        }

    }
}