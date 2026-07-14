using System;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace GameVault.Models
{
    public class Game : INotifyPropertyChanged
    {
        private int id;
        private string name = string.Empty;
        private string genre = string.Empty;
        private GameStatus status;
        private double? rating;
        private double? playtimeHours;
        private string? notes;

        private int? steamAppId;
        private bool isFromSteam;
        private string? steamImageUrl;
        private string? steamDescription;

        public int Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Genre
        {
            get => genre;
            set
            {
                genre = value;
                OnPropertyChanged(nameof(Genre));
            }
        }

        public GameStatus Status
        {
            get => status;
            set
            {
                status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public double? Rating
        {
            get => rating;
            set
            {
                rating = value;
                OnPropertyChanged(nameof(Rating));
            }
        }

        public double? PlaytimeHours
        {
            get => playtimeHours;
            set
            {
                playtimeHours = value;
                OnPropertyChanged(nameof(PlaytimeHours));
            }
        }

        public string? Notes
        {
            get => notes;
            set
            {
                notes = value;
                OnPropertyChanged(nameof(Notes));
            }
        }





        public int? SteamAppId
        {
            get => steamAppId;
            set
            {
                steamAppId = value;
                OnPropertyChanged(nameof(SteamAppId));
            }
        }

        public bool IsFromSteam
        {
            get => isFromSteam;
            set
            {
                isFromSteam = value;
                OnPropertyChanged(nameof(IsFromSteam));
            }
        }

        public string? SteamImageUrl
        {
            get => steamImageUrl;
            set
            {
                steamImageUrl = value;
                OnPropertyChanged(nameof(SteamImageUrl));
            }
        }

        public string? SteamDescription
        {
            get => steamDescription;
            set
            {
                steamDescription = value;
                OnPropertyChanged(nameof(SteamDescription));
            }
        }

        private string? steamGenres;

        public string? SteamGenres
        {
            get => steamGenres;
            set
            {
                steamGenres = value;
                OnPropertyChanged(nameof(SteamGenres));
            }
        }


        public string? Developer { get; set; }

        public string? Publisher { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public string? ExecutablePath { get; set; }

        public bool CanPlay => SteamAppId.HasValue || !string.IsNullOrWhiteSpace(ExecutablePath);

        private bool needsInstall;

        [JsonIgnore]
        public bool NeedsInstall
        {
            get => needsInstall;
            set
            {
                needsInstall = value;
                OnPropertyChanged(nameof(NeedsInstall));
            }
        }

        private int? achievementsUnlocked;

        public int? AchievementsUnlocked
        {
            get => achievementsUnlocked;
            set
            {
                achievementsUnlocked = value;
                OnPropertyChanged(nameof(AchievementsUnlocked));
                OnPropertyChanged(nameof(AchievementsDisplay));
                OnPropertyChanged(nameof(AchievementsPercent));
            }
        }

        private int? achievementsTotal;

        public int? AchievementsTotal
        {
            get => achievementsTotal;
            set
            {
                achievementsTotal = value;
                OnPropertyChanged(nameof(AchievementsTotal));
                OnPropertyChanged(nameof(AchievementsDisplay));
                OnPropertyChanged(nameof(AchievementsPercent));
            }
        }

        public string AchievementsDisplay =>
            AchievementsTotal is int total && total > 0 && AchievementsUnlocked is int unlocked
                ? $"{unlocked}/{total} ({Math.Round(unlocked * 100.0 / total)}%)"
                : "—";

        public double AchievementsPercent =>
            AchievementsTotal is int total && total > 0 && AchievementsUnlocked is int unlocked
                ? unlocked * 100.0 / total
                : 0;



        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}