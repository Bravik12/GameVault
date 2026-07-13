using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using GameVault.Models;

namespace GameVault.Services
{
    public class SteamService
    {
        private readonly HttpClient httpClient;

        public SteamService()
        {
            httpClient = new HttpClient();
        }


        public async Task<string> TestConnection()
        {
            var url =
                "https://api.steampowered.com/ISteamWebAPIUtil/GetSupportedAPIList/v1/";

            var response = await httpClient.GetStringAsync(url);

            return response;
        }


        public async Task<SteamGameInfo?> GetGameInfo(int appId)
        {
            var url =
                $"https://store.steampowered.com/api/appdetails?appids={appId}";

            var json = await httpClient.GetStringAsync(url);

            using var document = JsonDocument.Parse(json);

            var root = document.RootElement;
            
            var gameData = root
                .GetProperty(appId.ToString())
                .GetProperty("data");


            return new SteamGameInfo
            {
                Name = gameData.GetProperty("name").GetString() ?? "",
                HeaderImage = gameData.GetProperty("header_image").GetString(),
                ShortDescription = gameData.GetProperty("short_description").GetString()
            };
        }

    }
}