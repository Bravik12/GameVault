using System;
using System.Collections.Generic;
using System.Text;

namespace GameVault.Models
{
    public class SteamOwnedGame
    {
        public int AppId { get; set; }
        
        public string Name { get; set; } = string.Empty;

        public double PlaytimeHours { get; set; }
    }
}