using System.Collections.Generic;
using System.Collections.ObjectModel;
using MemoryMatch.Models;
using MemoryMatch.Services;

namespace MemoryMatch.ViewModels
{

    public class StatisticsViewModel : ViewModelBase
    {
        private ObservableCollection<UserStatistics> _statistics;
        
        private readonly UserService _userService;

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
        public StatisticsViewModel()
        {
            _userService = new UserService();
            LoadStatistics();
        }
        public void LoadStatistics()
        {
            List<User> users = _userService.LoadUsers();
            
            List<UserStatistics> statsList = new List<UserStatistics>();
            
            foreach (User user in users)
            {
                int gamesWon = 0;
                
                foreach (GameStatistic statistic in user.Statistics)
                {
                    if (statistic.IsWon)
                    {
                        gamesWon++;
                    }
                }
                
                UserStatistics userStats = new UserStatistics
                {
                    Username = user.Username,
                    GamesPlayed = user.Statistics.Count, 
                    GamesWon = gamesWon               
                };
                
                statsList.Add(userStats);
            }

            Statistics = new ObservableCollection<UserStatistics>(statsList);
        }
    }

    public class UserStatistics
    {
        public string Username { get; set; }
        
        public int GamesPlayed { get; set; }
        
        public int GamesWon { get; set; }
    }
} 