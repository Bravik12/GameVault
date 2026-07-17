using System;
using System.Collections.Generic;

namespace GameVault.Models
{
    public class GameList
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;

        public List<int> GameIds { get; set; } = new();

        public override string ToString()
        {
            return Name;
        }
    }
}
