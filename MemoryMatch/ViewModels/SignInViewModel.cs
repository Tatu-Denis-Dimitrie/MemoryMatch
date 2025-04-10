using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using MemoryMatch.Models;
using MemoryMatch.Helpers;

namespace MemoryMatch.ViewModels
{
    public class SignInViewModel : INotifyPropertyChanged
    {
        private readonly UserManager _userManager;
        private User _selectedUser;
        private string _newUsername;
        private string _selectedImagePath;
        private BitmapImage _previewImage;
        private readonly RelayCommand _createUserCommand;
        private readonly RelayCommand _deleteUserCommand;
        private readonly RelayCommand _playCommand;

        public ObservableCollection<User> Users 
        { 
            get 
            { 
                return _userManager.Users; 
            }
        }

        public User SelectedUser
        {
            get 
            { 
                return _selectedUser; 
            }
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsUserSelected));
                    OnPropertyChanged(nameof(CanDeleteUser));
                    OnPropertyChanged(nameof(CanPlay));
                    _deleteUserCommand.RaiseCanExecuteChanged();
                    _playCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string NewUsername
        {
            get 
            { 
                return _newUsername; 
            }
            set
            {
                if (_newUsername != value)
                {
                    _newUsername = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanCreateUser));
                    _createUserCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string SelectedImagePath
        {
            get 
            { 
                return _selectedImagePath; 
            }
            set
            {
                if (_selectedImagePath != value)
                {
                    _selectedImagePath = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanCreateUser));
                    _createUserCommand.RaiseCanExecuteChanged();
                    
                    if (!string.IsNullOrEmpty(value))
                    {
                        PreviewImage = new BitmapImage(new Uri(value));
                    }
                    else
                    {
                        PreviewImage = null;
                    }
                }
            }
        }

        public BitmapImage PreviewImage
        {
            get 
            { 
                return _previewImage; 
            }
            set
            {
                if (_previewImage != value)
                {
                    _previewImage = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsUserSelected
        {
            get
            {
                return SelectedUser != null;
            }
        }

        public bool CanDeleteUser
        {
            get
            {
                return SelectedUser != null;
            }
        }

        public bool CanPlay
        {
            get
            {
                return SelectedUser != null;
            }
        }

        public bool CanCreateUser
        {
            get
            {
                bool hasUsername = NewUsername != null && NewUsername.Trim().Length > 0;
                bool hasImage = SelectedImagePath != null && SelectedImagePath.Trim().Length > 0;
                return hasUsername && hasImage;
            }
        }

        public ICommand SelectImageCommand { get; }
        
        public ICommand CreateUserCommand 
        { 
            get 
            { 
                return _createUserCommand; 
            }
        }
        
        public ICommand DeleteUserCommand 
        { 
            get 
            { 
                return _deleteUserCommand; 
            }
        }
        
        public ICommand PlayCommand 
        { 
            get 
            { 
                return _playCommand; 
            }
        }

        public SignInViewModel()
        {
            _userManager = new UserManager();
            
            SelectImageCommand = new RelayCommand(ExecuteSelectImage);
            _createUserCommand = new RelayCommand(ExecuteCreateUser, CanExecuteCreateUser);
            _deleteUserCommand = new RelayCommand(ExecuteDeleteUser, CanExecuteDeleteUser);
            _playCommand = new RelayCommand(ExecutePlay, CanExecutePlay);
        }

        private void ExecuteSelectImage(object parameter)
        {
            string imageDirectory = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\Images\ProfilePictures"));

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Selectează o imagine",
                Filter = "Fișiere imagine|*.jpg;*.jpeg;*.png;*.gif|Toate fișierele|*.*",
                InitialDirectory = imageDirectory
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SelectedImagePath = openFileDialog.FileName;
            }
        }


        private void ExecuteCreateUser(object parameter)
        {
            if (CanCreateUser)
            {
                User newUser = new User(NewUsername, SelectedImagePath);
                _userManager.AddUser(newUser);
                NewUsername = string.Empty;
                SelectedImagePath = string.Empty;
                PreviewImage = null;
            }
        }

        private bool CanExecuteCreateUser(object parameter)
        {
            return CanCreateUser;
        }

        private void ExecuteDeleteUser(object parameter)
        {
            if (CanDeleteUser)
            {
                _userManager.DeleteUser(SelectedUser);
                SelectedUser = null;
            }
        }

        private bool CanExecuteDeleteUser(object parameter)
        {
            return CanDeleteUser;
        }

        private void ExecutePlay(object parameter)
        {
            var gameView = new Views.GameView(SelectedUser.Username);
            gameView.Show();
            
            foreach (Window window in Application.Current.Windows)
            {
                if (window is Views.SignInView)
                {
                    window.Close();
                    break;
                }
            }
        }

        private bool CanExecutePlay(object parameter)
        {
            return CanPlay;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
} 