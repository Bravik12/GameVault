using System;
using System.Collections.Generic;
using GameVault.Models;
using System.Windows.Input;
using GameVault.Commands;
using System.Windows;

namespace GameVault.ViewModels
{
    public class AddGameViewModel : ViewModelBase
    {
        public event EventHandler<Game>? GameAdded;

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

                if (isNotRated)
                {
                    Rating = null;
                }
                else if (Rating == null)
                {
                    Rating = 5;
                }

                OnPropertyChanged(nameof(IsNotRated));
            }
        }

        private string? validationMessage;

        public string? ValidationMessage
        {
            get => validationMessage;
            set
            {
                validationMessage = value;
                OnPropertyChanged(nameof(ValidationMessage));
            }
        }


        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                ValidationMessage = "Name is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Genre))
            {
                ValidationMessage = "Genre is required.";
                return false;
            }

            ValidationMessage = null;
            return true;

        }




        public double? PlaytimeHours { get; set; }

        public string? Notes { get; set; } = string.Empty;

        public IEnumerable<GameStatus> AvailableStatuses { get; }

        public ICommand AddGameCommand { get; }



        public AddGameViewModel()
        {
            //Name = "Test Game";

            AvailableStatuses = Enum.GetValues<GameStatus>();

            Rating = 5;
            IsNotRated = false;

            AddGameCommand = new RelayCommand(AddGame);
        }

        private void AddGame()
        {
            if (!Validate())
            {
                MessageBox.Show(
                    ValidationMessage,
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var game = new Game
            {
                Name = this.Name,
                Genre = this.Genre,
                Status = this.Status,
                Rating = this.IsNotRated ? null : this.Rating,
                PlaytimeHours = this.PlaytimeHours,
                Notes = this.Notes
            };

            GameAdded?.Invoke(this, game);
        }



    }
}
