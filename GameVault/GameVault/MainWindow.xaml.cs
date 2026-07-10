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
using System.ComponentModel;
using System.Collections.Specialized;

namespace GameVault
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<Game> Games { get; }

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

        public MainWindow()
        {
            InitializeComponent();

            Games = new ObservableCollection<Game>();

            HasGames = Games.Count > 0;

            DataContext = this;

            Games.CollectionChanged += Games_CollectionChanged;

        }


        private void AddGameButton_Click(object sender, RoutedEventArgs e)
        {
            var addGameWindow = new Views.AddGameWindow();

            addGameWindow.Owner = this;

            if (addGameWindow.ShowDialog() == true && addGameWindow.CreatedGame is Game game)
            {
                Games.Add(game);
            }
        }

        


    }
}