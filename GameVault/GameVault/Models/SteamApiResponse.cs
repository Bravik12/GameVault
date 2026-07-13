using System;
using System.Collections.Generic;
using System.Text;

namespace GameVault.Models
{
    public class SteamApiResponse
    {
        public bool Success { get; set; }

        public SteamGameData? Data { get; set; }
    }


    public class SteamGameData
    {
        public string Name { get; set; } = string.Empty;

        public string? HeaderImage { get; set; }

        public string? ShortDescription { get; set; }
    }
}