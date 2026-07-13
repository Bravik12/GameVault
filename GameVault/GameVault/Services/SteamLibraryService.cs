using GameVault.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;


namespace GameVault.Services
{
    public class SteamLibraryService
    {
        private readonly string apiKey;

        private readonly HttpClient httpClient = new HttpClient();

        public SteamLibraryService(string apiKey)
        {
            this.apiKey = apiKey;
        }



        public async Task<List<SteamOwnedGame>> GetOwnedGames(string steamId)
        {
            var url =
                $"https://api.steampowered.com/IPlayerService/GetOwnedGames/v1/" +
                $"?key={apiKey}&steamid={steamId}&include_appinfo=true";


            var response = await httpClient.GetStringAsync(url);


            using var json = JsonDocument.Parse(response);


            var games = new List<SteamOwnedGame>();


            var gameArray = json.RootElement
                .GetProperty("response")
                .GetProperty("games");


            foreach (var game in gameArray.EnumerateArray())
            {
                games.Add(new SteamOwnedGame
                {
                    AppId = game.GetProperty("appid").GetInt32(),

                    Name = game.GetProperty("name").GetString()
                           ?? string.Empty,

                    PlaytimeHours =
                        game.GetProperty("playtime_forever").GetDouble() / 60
                });
            }
            

            return games;
        }

        public async Task<SteamGameInfo?> GetGameInfo(int appId)
        {
            var url =
                $"https://store.steampowered.com/api/appdetails?appids={appId}";


            var response = await httpClient.GetStringAsync(url);

            using var json = JsonDocument.Parse(response);


            var app = json.RootElement
                .GetProperty(appId.ToString());


            if (!app.GetProperty("success").GetBoolean())
                return null;


            var data = app.GetProperty("data");


            var info = new SteamGameInfo
            {
                Name = data.GetProperty("name").GetString()
                       ?? string.Empty,

                HeaderImage = data.GetProperty("header_image").GetString(),

                ShortDescription =
                    data.GetProperty("short_description").GetString()
            };


            if (data.TryGetProperty("genres", out var genres))
            {
                foreach (var genre in genres.EnumerateArray())
                {
                    info.Genres.Add(
                        genre.GetProperty("description").GetString()
                        ?? string.Empty);
                }
            }

            if (data.TryGetProperty("developers", out var developers) &&
                developers.GetArrayLength() > 0)
            {
                info.Developer = developers[0].GetString();
            }

            if (data.TryGetProperty("publishers", out var publishers) &&
                publishers.GetArrayLength() > 0)
            {
                info.Publisher = publishers[0].GetString();
            }

            if (data.TryGetProperty("release_date", out var releaseDate) &&
                releaseDate.TryGetProperty("date", out var dateProperty) &&
                DateTime.TryParse(
                    dateProperty.GetString(),
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out var parsedDate))
            {
                info.ReleaseDate = parsedDate;
            }

            System.Diagnostics.Debug.WriteLine($"{info.Name}: {string.Join(", ", info.Genres)}");

            return info;
        }

        public async Task<(int Unlocked, int Total)?> GetAchievements(string steamId, int appId)
        {
            var url =
                $"https://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v1/" +
                $"?appid={appId}&key={apiKey}&steamid={steamId}";

            string response;

            try
            {
                response = await httpClient.GetStringAsync(url);
            }
            catch (HttpRequestException)
            {
                // Game has no achievements, or the Steam profile's game
                // details aren't public.
                return null;
            }

            using var json = JsonDocument.Parse(response);

            var playerStats = json.RootElement.GetProperty("playerstats");

            if (!playerStats.TryGetProperty("success", out var success) ||
                !success.GetBoolean() ||
                !playerStats.TryGetProperty("achievements", out var achievements))
            {
                return null;
            }

            int total = 0;
            int unlocked = 0;

            foreach (var achievement in achievements.EnumerateArray())
            {
                total++;

                if (achievement.GetProperty("achieved").GetInt32() == 1)
                {
                    unlocked++;
                }
            }

            return (unlocked, total);
        }

    }

}
