using System.ComponentModel;

namespace GameVault.Models
{
    public class GameSortOption
    {
        public string Name { get; set; } = string.Empty;

        public string? PropertyName { get; set; }

        public ListSortDirection Direction { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
