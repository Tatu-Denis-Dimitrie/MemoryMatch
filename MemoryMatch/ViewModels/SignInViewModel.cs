using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using MemoryMatch.Models;

namespace MemoryMatch.ViewModels
{
    public class SignInViewModel : INotifyPropertyChanged
    {
        private readonly UserManager _userManager;
        private User _selectedUser;
        private string _newUsername;
        private string _selectedImagePath;
        private BitmapImage _previewImage;

        public ObservableCollection<User> Users => _userManager.Users;

        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsUserSelected));
                    OnPropertyChanged(nameof(CanDeleteUser));
                    OnPropertyChanged(nameof(CanPlay));
                    DeleteUserCommand.RaiseCanExecuteChanged();
                    PlayCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string NewUsername
        {
            get => _newUsername;
            set
            {
                if (_newUsername != value)
                {
                    _newUsername = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanCreateUser));
                    CreateUserCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string SelectedImagePath
        {
            get => _selectedImagePath;
            set
            {
                if (_selectedImagePath != value)
                {
                    _selectedImagePath = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanCreateUser));
                    CreateUserCommand.RaiseCanExecuteChanged();
                    
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
            get => _previewImage;
            set
            {
                if (_previewImage != value)
                {
                    _previewImage = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsUserSelected => SelectedUser != null;
        public bool CanDeleteUser => SelectedUser != null;
        public bool CanPlay => SelectedUser != null;
        public bool CanCreateUser => !string.IsNullOrWhiteSpace(NewUsername) && !string.IsNullOrWhiteSpace(SelectedImagePath);

        public RelayCommand SelectImageCommand { get; }
        public RelayCommand CreateUserCommand { get; }
        public RelayCommand DeleteUserCommand { get; }
        public RelayCommand PlayCommand { get; }

        public SignInViewModel()
        {
            _userManager = new UserManager();
            
            SelectImageCommand = new RelayCommand(ExecuteSelectImage);
            CreateUserCommand = new RelayCommand(ExecuteCreateUser, CanExecuteCreateUser);
            DeleteUserCommand = new RelayCommand(ExecuteDeleteUser, CanExecuteDeleteUser);
            PlayCommand = new RelayCommand(ExecutePlay, CanExecutePlay);
        }

        private void ExecuteSelectImage(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Selectează o imagine",
                Filter = "Fișiere imagine|*.jpg;*.jpeg;*.png;*.gif|Toate fișierele|*.*"
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
            // TODO: Implementare pentru începerea jocului
            MessageBox.Show($"Jocul începe pentru utilizatorul {SelectedUser.Username}!");
        }

        private bool CanExecutePlay(object parameter)
        {
            return CanPlay;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
} 