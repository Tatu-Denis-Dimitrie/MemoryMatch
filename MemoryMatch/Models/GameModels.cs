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
    public class Card
    {
        public int Id { get; set; }
        public int PairId { get; set; }
        public string ImagePath { get; set; }
        public bool IsFlipped { get; set; }
        public bool IsMatched { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }
} 