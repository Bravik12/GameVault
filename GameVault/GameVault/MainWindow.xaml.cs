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

namespace GameVault
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Game> Games { get; }


        public MainWindow()
        {
            InitializeComponent();

            Games = new ObservableCollection<Game>();

            LoadSampleGames();

            DataContext = this;

        }

        private void AddGameButton_Click(object sender, RoutedEventArgs e)
        {
            var addGameWindow = new Views.AddGameWindow();

            addGameWindow.Owner = this;

            addGameWindow.ShowDialog();

        }

        private void LoadSampleGames()
        {
            Games.Add(new Game
            {
                Name = "The Witcher 3",
                Genre = "RPG",
                Status = GameStatus.Playing,
                Rating = 10,
                PlaytimeHours = 120.5,
                Notes = "An amazing open-world experience!"
            });
        }


    }
}