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
                Games.Remove(SelectedGame);

                storageService.SaveGames(Games);
            }
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