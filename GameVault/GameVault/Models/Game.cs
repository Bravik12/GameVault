using System;
using System.Collections.Generic;
using System.Text;

namespace GameVault.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public GameStatus Status { get; set; }
        public double? Rating { get; set; }
        public double? PlaytimeHours { get; set; }
        public string? Notes { get; set; }


    }
}
