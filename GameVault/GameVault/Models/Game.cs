using System;
using System.Collections.Generic;
using System.Text;

namespace GameVault.Models
{
    internal class Game
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Genre { get; set; }
        public GameStatus Status { get; set; }
        public int? Rating { get; set; }
        public double? PlaytimeHours { get; set; }
        public string? Notes { get; set; }


    }
}
