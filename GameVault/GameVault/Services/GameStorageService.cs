using System;
using System.Collections.Generic;
using System.Text;
using GameVault.Models;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace GameVault.Services
{
    public class GameStorageService
    {

        private readonly string FilePath;

        public void SaveGames(IEnumerable<Game> games)
        {
            var json = JsonSerializer.Serialize(games, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });

            File.WriteAllText(FilePath, json);
        }

        public List<Game> LoadGames()
        {
            if (!File.Exists(FilePath))
            {
                return new List<Game>();
            }

            try
            {
                var json = File.ReadAllText(FilePath);

                return JsonSerializer.Deserialize<List<Game>>(json)
                       ?? new List<Game>();
            }
            catch (JsonException)
            {
                return new List<Game>();
            }
        }

        public GameStorageService()
        {
            var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GameVault");

            Directory.CreateDirectory(folderPath);

            FilePath = Path.Combine(folderPath, "games.json");

        }

    }
}
