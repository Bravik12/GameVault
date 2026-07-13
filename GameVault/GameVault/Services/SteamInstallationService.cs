using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace GameVault.Services
{
    public class SteamInstallationService
    {
        public bool IsGameInstalled(int appId)
        {
            try
            {
                foreach (var libraryFolder in GetLibraryFolders())
                {
                    var manifestPath = Path.Combine(
                        libraryFolder,
                        "steamapps",
                        $"appmanifest_{appId}.acf");

                    if (File.Exists(manifestPath))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                // If Steam isn't installed, or its files are in an
                // unexpected shape, treat the game as not installed.
            }

            return false;
        }

        private IEnumerable<string> GetLibraryFolders()
        {
            var steamPath = GetSteamPath();

            if (string.IsNullOrEmpty(steamPath))
            {
                yield break;
            }

            yield return steamPath;

            var libraryFoldersFile = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");

            if (!File.Exists(libraryFoldersFile))
            {
                yield break;
            }

            var content = File.ReadAllText(libraryFoldersFile);

            foreach (Match match in Regex.Matches(content, "\"path\"\\s+\"([^\"]+)\""))
            {
                yield return match.Groups[1].Value.Replace(@"\\", @"\");
            }
        }

        private string? GetSteamPath()
        {
            var path = Registry.CurrentUser
                .OpenSubKey(@"Software\Valve\Steam")?
                .GetValue("SteamPath") as string;

            if (!string.IsNullOrEmpty(path))
            {
                return path;
            }

            return Registry.LocalMachine
                .OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam")?
                .GetValue("InstallPath") as string;
        }
    }
}
