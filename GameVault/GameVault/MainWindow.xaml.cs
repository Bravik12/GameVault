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

namespace GameVault
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddGameButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Add Game button clicked! This will open the AddGameWindow.");
            // Open the AddGameWindow when the button is clicked
            // AddGameWindow addGameWindow = new AddGameWindow();
            // addGameWindow.ShowDialog();
        }

    }
}