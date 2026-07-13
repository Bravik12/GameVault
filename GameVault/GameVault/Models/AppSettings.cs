using System;
using System.Collections.Generic;
using System.Text;

namespace GameVault.Models
{
    public class AppSettings
    {
        public string SteamId { get; set; } = string.Empty;

        public string SteamApiKey { get; set; } = string.Empty;

        public List<int> IgnoredSteamAppIds { get; set; } = new();
    }
}
