using System;
using System.Collections.Generic;
using GameVault.Models;
using System.IO;
using System.Text.Json;

namespace GameVault.Services
{
    public class GameListStorageService
    {
        private readonly string filePath;

        public GameListStorageService()
        {
            var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GameVault");

            Directory.CreateDirectory(folderPath);

            filePath = Path.Combine(folderPath, "lists.json");
        }

        public void SaveLists(IEnumerable<GameList> lists)
        {
            var json = JsonSerializer.Serialize(lists, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(filePath, json);
        }

        public List<GameList> LoadLists()
        {
            if (!File.Exists(filePath))
            {
                return new List<GameList>();
            }

            try
            {
                var json = File.ReadAllText(filePath);

                return JsonSerializer.Deserialize<List<GameList>>(json)
                       ?? new List<GameList>();
            }
            catch (JsonException)
            {
                return new List<GameList>();
            }
        }
    }
}
