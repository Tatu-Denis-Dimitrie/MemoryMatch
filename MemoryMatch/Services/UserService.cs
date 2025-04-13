using MemoryMatch.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Diagnostics;

namespace MemoryMatch.Services
{
    public class UserService
    {
        private readonly string _usersFilePath;

        public UserService()
        {
            _usersFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json");
        }

        public List<User> LoadUsers()
        {
            if (!File.Exists(_usersFilePath))
            {
                return new List<User>();
            }

            try
            {
                string json = File.ReadAllText(_usersFilePath);
                
                var options = new JsonSerializerOptions 
                { 
                    IncludeFields = true,
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<List<User>>(json, options) ?? new List<User>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Eroare la incarcarea utilizatorilor: {ex.Message}");
                return new List<User>();
            }
        }

        public void SaveUsers(List<User> users)
        {
            try
            {
                var options = new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    IncludeFields = true,
                    PropertyNameCaseInsensitive = true
                };
                
                string json = JsonSerializer.Serialize(users, options);
                
                File.WriteAllText(_usersFilePath, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Eroare la salvarea utilizatorilor: {ex.Message}");
            }
        }

        public void AddUser(User user)
        {
            var users = LoadUsers();
            users.Add(user);
            SaveUsers(users);
        }

        public void DeleteUser(string username)
        {
            var users = LoadUsers();
            
            User userToRemove = null;
            foreach (var user in users)
            {
                if (user.Username == username)
                {
                    userToRemove = user;
                    break;
                }
            }
            
            if (userToRemove != null)
            {
                GameService gameService = new GameService();
                gameService.DeleteSavedGames(username);

                users.Remove(userToRemove);
                SaveUsers(users);
            }
        }

        public User GetUser(string username)
        {
            var users = LoadUsers();
            
            foreach (var user in users)
            {
                if (user.Username == username)
                {
                    return user;
                }
            }
            
            return null;
        }
    }
} 