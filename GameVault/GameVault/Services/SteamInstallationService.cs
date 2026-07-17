using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace GameVault.Services
{
    public class SteamInstallationService
    {
        private List<string>? cachedLibraryFolders;

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

        private List<string> GetLibraryFolders()
        {
            if (cachedLibraryFolders != null)
            {
                return cachedLibraryFolders;
            }

            var folders = new List<string>();

            var steamPath = GetSteamPath();

            if (string.IsNullOrEmpty(steamPath))
            {
                cachedLibraryFolders = folders;
                return folders;
            }

            folders.Add(steamPath);

            var libraryFoldersFile = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");

            if (File.Exists(libraryFoldersFile))
            {
                var content = File.ReadAllText(libraryFoldersFile);

                foreach (Match match in Regex.Matches(content, "\"path\"\\s+\"([^\"]+)\""))
                {
                    folders.Add(match.Groups[1].Value.Replace(@"\\", @"\"));
                }
            }

            cachedLibraryFolders = folders;
            return folders;
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
