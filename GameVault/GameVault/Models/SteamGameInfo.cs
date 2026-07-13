using System;
using System.Collections.Generic;
using System.Text;

namespace GameVault.Models
{
    public class SteamGameInfo
    {
        public string Name { get; set; } = string.Empty;

        public string? HeaderImage { get; set; }
        
        public string? ShortDescription { get; set; }

        public List<string> Genres { get; set; } = new();
    }
}