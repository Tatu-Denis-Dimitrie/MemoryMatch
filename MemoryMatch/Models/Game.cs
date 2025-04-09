using System;
using System.Collections.Generic;

namespace MemoryMatch.Models
{
    public class Game
    {
        public string Username { get; set; }
        public string Category { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public TimeSpan RemainingTime { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public List<Card> Cards { get; set; } = new List<Card>();
    }
} 