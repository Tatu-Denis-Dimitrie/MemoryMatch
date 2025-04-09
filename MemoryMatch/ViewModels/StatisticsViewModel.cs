using System.Collections.Generic;
using System.Collections.ObjectModel;
using MemoryMatch.Models;
using MemoryMatch.Services;

namespace MemoryMatch.ViewModels
{
    /// <summary>
    /// Model de vizualizare pentru afișarea statisticilor jucătorilor
    /// </summary>
    public class StatisticsViewModel : ViewModelBase
    {
        /// <summary>
        /// Colecția de statistici ale utilizatorilor
        /// </summary>
        private ObservableCollection<UserStatistics> _statistics;
        
        /// <summary>
        /// Serviciul pentru accesarea datelor utilizatorilor
        /// </summary>
        private readonly UserService _userService;

        /// <summary>
        /// Proprietate pentru accesarea statisticilor
        /// </summary>
        public ObservableCollection<UserStatistics> Statistics
        {
            get 
            { 
                return _statistics; 
            }
            set 
            { 
                SetProperty(ref _statistics, value); 
            }
        }

        /// <summary>
        /// Constructor: inițializează serviciul și încarcă statisticile
        /// </summary>
        public StatisticsViewModel()
        {
            // Creez instanța serviciului de utilizatori
            _userService = new UserService();
            // Încarc statisticile utilizatorilor
            LoadStatistics();
        }

        /// <summary>
        /// Încarcă statisticile tuturor utilizatorilor din sistem
        /// </summary>
        public void LoadStatistics()
        {
            // Obțin lista de utilizatori din serviciu
            List<User> users = _userService.LoadUsers();
            
            // Creez o listă nouă pentru a stoca statisticile calculate
            List<UserStatistics> statsList = new List<UserStatistics>();
            
            // Procesez fiecare utilizator pentru a-i calcula statisticile
            foreach (User user in users)
            {
                // Numărul de jocuri câștigate - trebuie calculat manual
                int gamesWon = 0;
                
                // Parcurg toate statisticile jocurilor utilizatorului
                foreach (GameStatistic statistic in user.Statistics)
                {
                    // Dacă jocul a fost câștigat, incrementez contorul
                    if (statistic.IsWon)
                    {
                        gamesWon++;
                    }
                }
                
                // Creez un obiect nou cu statisticile calculate pentru utilizator
                UserStatistics userStats = new UserStatistics
                {
                    Username = user.Username,
                    GamesPlayed = user.Statistics.Count,  // Numărul total de jocuri
                    GamesWon = gamesWon                  // Numărul de jocuri câștigate
                };
                
                // Adaug statisticile utilizatorului în lista generală
                statsList.Add(userStats);
            }

            // Actualizez proprietatea Statistics cu noile date
            Statistics = new ObservableCollection<UserStatistics>(statsList);
        }
    }

    /// <summary>
    /// Clasă model pentru stocarea statisticilor unui utilizator
    /// Conține informații despre numărul de jocuri jucate și câștigate
    /// </summary>
    public class UserStatistics
    {
        /// <summary>
        /// Numele de utilizator
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// Numărul total de jocuri jucate de utilizator
        /// </summary>
        public int GamesPlayed { get; set; }
        
        /// <summary>
        /// Numărul de jocuri câștigate de utilizator
        /// </summary>
        public int GamesWon { get; set; }
    }
} 