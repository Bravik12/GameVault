using GameVault.Models;

namespace GameVault.ViewModels
{
    public class GameListMembershipItem : ViewModelBase
    {
        public GameList List { get; }

        public string Name => List.Name;

        private bool isChecked;

        public bool IsChecked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public GameListMembershipItem(GameList list, bool isChecked)
        {
            List = list;
            this.isChecked = isChecked;
        }
    }
}
