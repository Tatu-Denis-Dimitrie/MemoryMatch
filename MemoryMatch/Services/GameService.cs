using MemoryMatch.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MemoryMatch.Services
{
    public class GameService
    {
        private readonly string _gamesDirectory;

        public GameService()
        {
            _gamesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SavedGames");
            
            if (!Directory.Exists(_gamesDirectory))
            {
                Directory.CreateDirectory(_gamesDirectory);
            }
        }

        public void SaveGame(Game game)
        {
            string filePath = Path.Combine(_gamesDirectory, $"{game.Username}_game.json");
            
            string json = JsonSerializer.Serialize(game, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            
            File.WriteAllText(filePath, json);
        }

        public Game LoadGame(string username)
        {
            string filePath = Path.Combine(_gamesDirectory, $"{username}_game.json");
            
            if (!File.Exists(filePath))
            {
                return null;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<Game>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void DeleteSavedGames(string username)
        {
            string filePath = Path.Combine(_gamesDirectory, $"{username}_game.json");
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public Game CreateNewGame(string category, int rows, int columns, TimeSpan gameTime, string username)
        {
            Game game = new Game
            {
                Category = category,
                Rows = rows,
                Columns = columns,
                RemainingTime = gameTime,
                ElapsedTime = TimeSpan.Zero,
                Username = username,
                Cards = GenerateCards(category, rows, columns)
            };

            return game;
        }

        private List<Card> GenerateCards(string category, int rows, int columns)
        {
            List<Card> cards = new List<Card>();
            int totalCards = rows * columns;
            int totalPairs = totalCards / 2;

            string[] imagePaths = GetImagePathsForCategory(category, totalPairs);

            List<int> positions = new List<int>();
            for (int i = 0; i < totalCards; i++)
            {
                positions.Add(i);
            }
            
            Random random = new Random();
            for (int i = positions.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                int temp = positions[i];
                positions[i] = positions[j];
                positions[j] = temp;
            }

            for (int i = 0; i < totalPairs; i++)
            {
                int pos1 = positions[0];
                positions.RemoveAt(0);
                int row1 = pos1 / columns;
                int col1 = pos1 % columns;

                int pos2 = positions[0];
                positions.RemoveAt(0);
                int row2 = pos2 / columns;
                int col2 = pos2 % columns;

                Card card1 = new Card
                {
                    Id = pos1,
                    PairId = i,
                    ImagePath = imagePaths[i],
                    IsFlipped = false,
                    IsMatched = false,
                    Row = row1,
                    Column = col1
                };
                cards.Add(card1);

                Card card2 = new Card
                {
                    Id = pos2,
                    PairId = i,
                    ImagePath = imagePaths[i],
                    IsFlipped = false,
                    IsMatched = false,
                    Row = row2,
                    Column = col2
                };
                cards.Add(card2);
            }

            cards.Sort((a, b) => a.Id.CompareTo(b.Id));
            return cards;
        }

        private string[] GetImagePathsForCategory(string category, int count)
        {
            string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "GameCategories", category);
            
            if (Directory.Exists(basePath))
            {
                List<string> imageFiles = new List<string>();
                
                string[] jpgFiles = Directory.GetFiles(basePath, "*.jpg");
                foreach (string file in jpgFiles)
                {
                    imageFiles.Add(file);
                }
                
                string[] pngFiles = Directory.GetFiles(basePath, "*.png");
                foreach (string file in pngFiles)
                {
                    imageFiles.Add(file);
                }
                
                string[] gifFiles = Directory.GetFiles(basePath, "*.gif");
                foreach (string file in gifFiles)
                {
                    imageFiles.Add(file);
                }

                if (imageFiles.Count >= count)
                {
                    string[] result = new string[count];
                    for (int i = 0; i < count; i++)
                    {
                        result[i] = imageFiles[i];
                    }
                    return result;
                }
            }

            string[] defaultPaths = new string[count];
            for (int i = 0; i < count; i++)
            {
                defaultPaths[i] = $"Resources/GameCategories/{category}/image{i + 1}.jpg";
            }
            return defaultPaths;
        }

        public void SaveStatistic(string username, GameStatistic statistic)
        {
            try
            {
                UserService userService = new UserService();
                var users = userService.LoadUsers();
                
                User userToUpdate = null;
                foreach (var user in users)
                {
                    if (user.Username == username)
                    {
                        userToUpdate = user;
                        break;
                    }
                }
                
                if (userToUpdate != null)
                {
                    userToUpdate.Statistics.Add(statistic);
                    userService.SaveUsers(users);
                }
            }
            catch (Exception)
            {}
        }
    }
} 