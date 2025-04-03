using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MemoryMatch.Models
{
    public class User : INotifyPropertyChanged
    {
        private string _username;
        private string _imagePath;
        public List<GameStatistic> Statistics { get; set; } = new List<GameStatistic>();

        public string Username
        {
            get { return _username; }
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                if (_imagePath != value)
                {
                    _imagePath = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public User()
        {
        }

        public User(string username, string imagePath)
        {
            Username = username;
            ImagePath = imagePath;
        }
    }

    public class GameStatistic
    {
        public DateTime Date { get; set; }
        public bool IsWon { get; set; }
        public string Category { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public TimeSpan TimeToComplete { get; set; }

        public GameStatistic()
        {
            Date = DateTime.Now;
        }
    }
} 