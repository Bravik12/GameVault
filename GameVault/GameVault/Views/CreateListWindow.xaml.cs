using System.Windows;
using System.Windows.Input;

namespace GameVault.Views
{
    public partial class CreateListWindow : Window
    {
        public string ListName { get; private set; } = string.Empty;

        public CreateListWindow()
        {
            InitializeComponent();

            NameTextBox.Focus();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            TryCreate();
        }

        private void NameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TryCreate();
            }
        }

        private void TryCreate()
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("List name cannot be empty.", "GameVault");
                return;
            }

            ListName = NameTextBox.Text.Trim();

            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
