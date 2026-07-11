using System;
using System.Collections.Generic;
using System.Text;

namespace GameVault.Models
{
    public class GameStatusFilter
    {
        public string Name { get; set; } = string.Empty;

        public GameStatus? Status { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}