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



        public MainWindow()
        {
            InitializeComponent();

            storageService = new GameStorageService();

            Games = new ObservableCollection<Game>();

            Games.CollectionChanged += (s, e) =>
            {
                HasGames = Games.Count > 0;
            };
            
            DataContext = this;

            LoadGames();

            Games.CollectionChanged += Games_CollectionChanged;

        }


 



    }
}