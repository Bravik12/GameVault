using System;
using System.Collections.Generic;
using GameVault.Models;

namespace GameVault.ViewModels
{
    public class AddGameViewModel : ViewModelBase
    {
        public string Name { get; set; } = string.Empty;

        public string Genre { get; set; } = string.Empty;

        public GameStatus Status { get; set; }

        private double? rating = 5;

        public double? Rating 
        {
            get => rating;
            set 
            {  
                rating = value; 
                OnPropertyChanged(nameof(Rating));
            }

        }

        private bool isNotRated;

        public bool IsNotRated
        {
            get => isNotRated;
            set
            {
                isNotRated = value;
                OnPropertyChanged(nameof(IsNotRated));
            }
        }

        public double? PlaytimeHours { get; set; }

        public string? Notes { get; set; } = string.Empty;

        public IEnumerable<GameStatus> AvailableStatuses { get; }

        

        public AddGameViewModel()
        {
            Name = "Test Game";

            AvailableStatuses = Enum.GetValues<GameStatus>();
        }



    }
}
