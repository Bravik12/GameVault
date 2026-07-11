using System;
using System.ComponentModel;

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


        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}