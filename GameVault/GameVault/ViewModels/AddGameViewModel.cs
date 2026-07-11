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

        private readonly Game? existingGame;

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




        private void AddGame()
        {
            Game game;

            if (existingGame != null)
            {
                game = existingGame;
            }
            else
            {
                game = new Game();

            }

            game.Name = Name;
            game.Genre = Genre;
            game.Status = Status;
            game.Rating = IsNotRated ? null : Rating;
            game.PlaytimeHours = PlaytimeHours;
            game.Notes = Notes;

            GameAdded?.Invoke(this, game);
        }




        public AddGameViewModel(Game? game = null)
        {
            existingGame = game;

            AvailableStatuses = Enum.GetValues<GameStatus>();
            
            AddGameCommand = new RelayCommand(AddGame);
            
            if (game != null)
            {
                Name = game.Name;
                Genre = game.Genre;
                Status = game.Status;
                Rating = game.Rating;
                PlaytimeHours = game.PlaytimeHours;
                Notes = game.Notes;

                IsNotRated = !game.Rating.HasValue;
            } else
            {
                Rating = 5;
                IsNotRated = false;
            }
            

            
        }





    }
}
