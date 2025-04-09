using System;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace MemoryMatch.Models
{
    /// <summary>
    /// Manager pentru gestionarea utilizatorilor aplicației
    /// </summary>
    public class UserManager
    {
        private static string UsersFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json");
        private ObservableCollection<User> _users;

        public ObservableCollection<User> Users
        {
            get
            {
                if (_users == null)
                {
                    LoadUsers();
                }
                return _users;
            }
        }

        public UserManager()
        {
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                if (File.Exists(UsersFilePath))
                {
                    string json = File.ReadAllText(UsersFilePath);
                    var userList = JsonSerializer.Deserialize<List<User>>(json);
                    _users = new ObservableCollection<User>(userList);
                }
                else
                {
                    _users = new ObservableCollection<User>();
                }
            }
            catch (Exception)
            {
                _users = new ObservableCollection<User>();
            }
        }

        public void SaveUsers()
        {
            try
            {
                string json = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(UsersFilePath, json);
            }
            catch (Exception)
            {
            }
        }

        public void AddUser(User user)
        {
            bool usernameExists = false;
            foreach (var existingUser in Users)
            {
                if (existingUser.Username == user.Username)
                {
                    usernameExists = true;
                    break;
                }
            }
            
            if (!usernameExists)
            {
                Users.Add(user);
                SaveUsers();
            }
        }

        public void DeleteUser(User user)
        {
            if (Users.Contains(user))
            {
                Users.Remove(user);
                SaveUsers();
            }
        }
    }
} 