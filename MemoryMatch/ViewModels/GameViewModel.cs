using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Windows.Threading;
using MemoryMatch.Views;
using MemoryMatch.Models;
using MemoryMatch.Services;
using System.Diagnostics;
using MemoryMatch.Helpers;

namespace MemoryMatch.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        private readonly GameService _gameService;
        private string _currentUsername;
        private string _selectedCategory;
        private int _boardRows = 4;
        private int _boardColumns = 4;
        private ObservableCollection<CardViewModel> _gameBoard;
        private Visibility _gameBoardVisibility = Visibility.Collapsed;
        private Visibility _gameAreaMessageVisibility = Visibility.Visible;
        private int _selectedRows = 4;
        private int _selectedColumns = 4;
        private ObservableCollection<int> _dimensionOptions;
        private int _gameTimeInSeconds = 60;
        private int _remainingTimeInSeconds;
        private string _timeDisplay;
        private DispatcherTimer _gameTimer;
        private CardViewModel _firstSelectedCard;
        private CardViewModel _secondSelectedCard;
        private bool _isProcessingCards;
        private int _pairsFound;
        private int _totalPairs;
        private ObservableCollection<int> _timeOptions;
        private int _selectedGameTime = 60;
        private ObservableCollection<string> _categories;

        public string CurrentUsername
        {
            get => _currentUsername;
            set => SetProperty(ref _currentUsername, value);
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        public int BoardRows
        {
            get => _boardRows;
            set => SetProperty(ref _boardRows, value);
        }

        public int BoardColumns
        {
            get => _boardColumns;
            set => SetProperty(ref _boardColumns, value);
        }

        public ObservableCollection<CardViewModel> GameBoard
        {
            get => _gameBoard;
            set => SetProperty(ref _gameBoard, value);
        }

        public Visibility GameBoardVisibility
        {
            get => _gameBoardVisibility;
            set => SetProperty(ref _gameBoardVisibility, value);
        }

        public Visibility GameAreaMessageVisibility
        {
            get => _gameAreaMessageVisibility;
            set => SetProperty(ref _gameAreaMessageVisibility, value);
        }

        public int SelectedRows
        {
            get => _selectedRows;
            set => SetProperty(ref _selectedRows, value);
        }

        public int SelectedColumns
        {
            get => _selectedColumns;
            set => SetProperty(ref _selectedColumns, value);
        }

        public ObservableCollection<int> DimensionOptions
        {
            get => _dimensionOptions;
            set => SetProperty(ref _dimensionOptions, value);
        }
        
        public ObservableCollection<int> TimeOptions
        {
            get => _timeOptions;
            set => SetProperty(ref _timeOptions, value);
        }
        
        public int SelectedGameTime
        {
            get => _selectedGameTime;
            set => SetProperty(ref _selectedGameTime, value);
        }
        
        public int RemainingTimeInSeconds
        {
            get => _remainingTimeInSeconds;
            set
            {
                if (SetProperty(ref _remainingTimeInSeconds, value))
                {
                    TimeDisplay = $"{value / 60:D2}:{value % 60:D2}";
                }
            }
        }
        
        public string TimeDisplay
        {
            get => _timeDisplay;
            set => SetProperty(ref _timeDisplay, value);
        }

        public ObservableCollection<string> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public ICommand SelectCategoryCommand { get; }
        public ICommand NewGameCommand { get; }
        public ICommand OpenGameCommand { get; }
        public ICommand SaveGameCommand { get; }
        public ICommand ShowStatisticsCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand SetStandardBoardCommand { get; }
        public ICommand SetCustomBoardCommand { get; }
        public ICommand ShowAboutCommand { get; }
        public ICommand CardClickCommand { get; }

        public GameViewModel()
        {
            _gameService = new GameService();
            
            GameBoard = new ObservableCollection<CardViewModel>();
            DimensionOptions = new ObservableCollection<int> { 2, 3, 4, 5, 6 };
            TimeOptions = new ObservableCollection<int> { 30, 60, 90, 120, 180, 300 };
            Categories = new ObservableCollection<string> { "Animale", "Peisaje", "Mancaruri" };

            SelectCategoryCommand = new RelayCommand(ExecuteSelectCategory);
            NewGameCommand = new RelayCommand(ExecuteNewGame);
            OpenGameCommand = new RelayCommand(ExecuteOpenGame);
            SaveGameCommand = new RelayCommand(ExecuteSaveGame);
            ShowStatisticsCommand = new RelayCommand(ExecuteShowStatistics);
            ExitCommand = new RelayCommand(ExecuteExit);
            SetStandardBoardCommand = new RelayCommand(ExecuteSetStandardBoard);
            SetCustomBoardCommand = new RelayCommand(ExecuteSetCustomBoard);
            ShowAboutCommand = new RelayCommand(ExecuteShowAbout);
            CardClickCommand = new RelayCommand(ExecuteCardClick, CanExecuteCardClick);
            
            _gameTimer = new DispatcherTimer();
            _gameTimer.Interval = TimeSpan.FromSeconds(1);
            _gameTimer.Tick += GameTimer_Tick;
            
            TimeDisplay = "00:00";
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            RemainingTimeInSeconds--;
            
            if (RemainingTimeInSeconds <= 0)
            {
                _gameTimer.Stop();
                GameLost();
            }
        }
        
        private void GameLost()
        {
            MessageBox.Show("Timpul a expirat! Jocul este pierdut.", "Timp expirat", MessageBoxButton.OK, MessageBoxImage.Information);
            
            _gameService.SaveStatistic(CurrentUsername, new GameStatistic
            {
                Category = SelectedCategory,
                IsWon = false,
                Rows = BoardRows,
                Columns = BoardColumns,
                TimeToComplete = TimeSpan.FromSeconds(SelectedGameTime)
            });
            
            _gameService.DeleteSavedGames(CurrentUsername);
            
            ResetGame(false);
        }
        
        private void GameWon()
        {
            _gameTimer.Stop();
            
            int timeUsed = SelectedGameTime - RemainingTimeInSeconds;
            
            MessageBox.Show($"Felicitări! Ai câștigat jocul în {timeUsed} secunde!", "Victorie", MessageBoxButton.OK, MessageBoxImage.Information);
            
            _gameService.SaveStatistic(CurrentUsername, new GameStatistic
            {
                Category = SelectedCategory,
                IsWon = true,
                Rows = BoardRows,
                Columns = BoardColumns,
                TimeToComplete = TimeSpan.FromSeconds(timeUsed)
            });
            
            _gameService.DeleteSavedGames(CurrentUsername);
            
            ResetGame(true);
        }
        
        private void ResetGame(bool wonGame)
        {
            _firstSelectedCard = null;
            _secondSelectedCard = null;
            _isProcessingCards = false;
            _pairsFound = 0;
            
            GameBoardVisibility = Visibility.Collapsed;
            GameAreaMessageVisibility = Visibility.Visible;
        }

        private void ExecuteSelectCategory(object parameter)
        {
            if (parameter is string category)
            {
                SelectedCategory = category;
                MessageBox.Show($"Categoria selectată: {SelectedCategory}");
            }
        }

        private void ExecuteNewGame(object parameter)
        {
            var newGameWindow = new Window
            {
                Title = "Joc nou",
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Width = 300
            };

            var mainStackPanel = new System.Windows.Controls.StackPanel { Margin = new Thickness(10) };

            mainStackPanel.Children.Add(new System.Windows.Controls.TextBlock 
            { 
                Text = "Selectați categoria:", 
                Margin = new Thickness(0, 0, 0, 5) 
            });

            var categoryComboBox = new System.Windows.Controls.ComboBox
            {
                ItemsSource = Categories,
                SelectedItem = SelectedCategory,
                Margin = new Thickness(0, 0, 0, 10),
                Width = 200
            };
            mainStackPanel.Children.Add(categoryComboBox);

            mainStackPanel.Children.Add(new System.Windows.Controls.TextBlock 
            { 
                Text = "Introduceți timpul limită (secunde):", 
                Margin = new Thickness(0, 0, 0, 5) 
            });

            var timeTextBox = new System.Windows.Controls.TextBox
            {
                Text = SelectedGameTime.ToString(),
                Margin = new Thickness(0, 0, 0, 10),
                Width = 200
            };
            mainStackPanel.Children.Add(timeTextBox);

            var buttonPanel = new System.Windows.Controls.StackPanel 
            { 
                Orientation = System.Windows.Controls.Orientation.Horizontal, 
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right 
            };
            
            var okButton = new System.Windows.Controls.Button 
            { 
                Content = "Începe jocul", 
                Width = 100, 
                Margin = new Thickness(0, 0, 5, 0) 
            };
            
            var cancelButton = new System.Windows.Controls.Button 
            { 
                Content = "Anulează", 
                Width = 75 
            };

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);
            mainStackPanel.Children.Add(buttonPanel);

            newGameWindow.Content = mainStackPanel;

            bool? dialogResult = false;

            okButton.Click += (s, e) =>
            {
                if (categoryComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Vă rugăm să selectați o categorie.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(timeTextBox.Text, out int seconds) || seconds <= 0)
                {
                    MessageBox.Show("Vă rugăm să introduceți un număr pozitiv de secunde.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (categoryComboBox.SelectedItem is string selectedCategory)
                {
                    SelectedCategory = selectedCategory;
                    SelectedGameTime = seconds;
                    dialogResult = true;
                    newGameWindow.Close();
                }
                else
                {
                    MessageBox.Show("Categorie invalidă.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            cancelButton.Click += (s, e) =>
            {
                dialogResult = false;
                newGameWindow.Close();
            };

            newGameWindow.ShowDialog();

            if (dialogResult != true)
            {
                return;
            }

            GameBoardVisibility = Visibility.Visible;
            GameAreaMessageVisibility = Visibility.Collapsed;

            _firstSelectedCard = null;
            _secondSelectedCard = null;
            _isProcessingCards = false;
            _pairsFound = 0;

            RemainingTimeInSeconds = SelectedGameTime;
            _gameTimer.Stop();

            InitializeGameBoard();

            _gameTimer.Start();
        }

        private void ExecuteOpenGame(object parameter)
        {
            if (string.IsNullOrEmpty(CurrentUsername))
            {
                MessageBox.Show("Nu există niciun utilizator selectat.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            Game savedGame = _gameService.LoadGame(CurrentUsername);
            
            if (savedGame == null)
            {
                MessageBox.Show("Nu există niciun joc salvat pentru utilizatorul curent.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            SelectedCategory = savedGame.Category;
            BoardRows = savedGame.Rows;
            BoardColumns = savedGame.Columns;
            
            RemainingTimeInSeconds = (int)savedGame.RemainingTime.TotalSeconds;
            
            GameBoardVisibility = Visibility.Visible;
            GameAreaMessageVisibility = Visibility.Collapsed;
            
            _firstSelectedCard = null;
            _secondSelectedCard = null;
            _isProcessingCards = false;
            _pairsFound = 0;
            
            LoadGameBoard(savedGame.Cards);
            
            _gameTimer.Stop();
            _gameTimer.Start();
            
            MessageBox.Show($"Joc încărcat. Timp rămas: {TimeDisplay}", "Joc încărcat", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExecuteSaveGame(object parameter)
        {
            if (GameBoardVisibility == Visibility.Collapsed)
            {
                MessageBox.Show("Nu există niciun joc activ pentru a fi salvat.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            _gameTimer.Stop();
            
            Game gameToSave = new Game
            {
                Username = CurrentUsername,
                Category = SelectedCategory,
                Rows = BoardRows,
                Columns = BoardColumns,
                RemainingTime = TimeSpan.FromSeconds(RemainingTimeInSeconds),
                ElapsedTime = TimeSpan.FromSeconds(SelectedGameTime - RemainingTimeInSeconds),
                Cards = new List<Card>()
            };
            
            foreach (var card in GameBoard)
            {
                gameToSave.Cards.Add(new Card
                {
                    Id = card.Id,
                    PairId = card.PairId,
                    ImagePath = card.FaceImagePath,
                    IsFlipped = card.IsFlipped,
                    IsMatched = card.IsMatched,
                    Row = card.Id / BoardColumns,
                    Column = card.Id % BoardColumns
                });
            }
            
            _gameService.SaveGame(gameToSave);
            
            MessageBox.Show($"Jocul curent a fost salvat. Timp rămas: {TimeDisplay}");
            
            _gameTimer.Start();
        }

        private void ExecuteShowStatistics(object parameter)
        {
            var statisticsWindow = new StatisticsWindow();
            statisticsWindow.ShowDialog();
        }

        private void ExecuteExit(object parameter)
        {
            _gameTimer.Stop();
            
            MessageBoxResult result = MessageBox.Show("Sigur doriți să vă întoarceți la ecranul de autentificare? Jocul curent va fi pierdut dacă nu l-ați salvat.", "Confirmare", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is GameView)
                    {
                        window.Close();
                        break;
                    }
                }
            }
            else
            {
                if (GameBoardVisibility == Visibility.Visible)
                {
                    _gameTimer.Start();
                }
            }
        }

        private void ExecuteSetStandardBoard(object parameter)
        {
            BoardRows = 4;
            BoardColumns = 4;
            MessageBox.Show("Dimensiune tablă: 4x4 (standard)");
        }

        private void ExecuteSetCustomBoard(object parameter)
        {
            if ((SelectedRows * SelectedColumns) % 2 != 0)
            {
                MessageBox.Show("Dimensiunile selectate rezultă într-un număr impar de cărți. Numărul de cărți trebuie să fie par.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            BoardRows = SelectedRows;
            BoardColumns = SelectedColumns;
            MessageBox.Show($"Dimensiune tablă: {BoardRows}x{BoardColumns} (personalizat)");
        }

        private void ExecuteShowAbout(object parameter)
        {
            MessageBox.Show(
                "Memory Match\n\n" +
                "Dezvoltat de: Tatu Denis-Dimitrie\n" +
                "Email: denis.tatu@student.unitbv.ro\n" +
                "Grupa: 10LF234\n" +
                "Specializare: Informatica",
                "Despre aplicație",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void ExecuteCardClick(object parameter)
        {
            if (parameter is CardViewModel card)
            {
                if (_isProcessingCards || card.IsFlipped || card.IsMatched)
                    return;
                
                card.IsFlipped = true;
                
                if (_firstSelectedCard == null)
                {
                    _firstSelectedCard = card;
                }
                else if (_secondSelectedCard == null && _firstSelectedCard != card)
                {
                    _secondSelectedCard = card;
                    _isProcessingCards = true;
                    
                    CheckForMatch();
                }
            }
        }
        
        private bool CanExecuteCardClick(object parameter)
        {
            return !_isProcessingCards && GameBoardVisibility == Visibility.Visible;
        }
        
        private void CheckForMatch()
        {
            if (_firstSelectedCard == null || _secondSelectedCard == null)
            {
                _isProcessingCards = false;
                return;
            }

            try
            {
                if (_firstSelectedCard.PairId == _secondSelectedCard.PairId)
                {
                    _firstSelectedCard.IsMatched = true;
                    _secondSelectedCard.IsMatched = true;
                    
                    _pairsFound++;
                    
                    if (_pairsFound >= _totalPairs)
                    {
                        GameWon();
                    }
                    else
                    {
                        ResetSelectedCards();
                    }
                }
                else
                {
                    var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                    
                    timer.Tick += (sender, args) =>
                    {
                        timer.Stop();
                        
                        try
                        {
                            if (_firstSelectedCard != null && _secondSelectedCard != null)
                            {
                                _firstSelectedCard.IsFlipped = false;
                                _secondSelectedCard.IsFlipped = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Eroare la resetarea cărților: {ex.Message}");
                        }
                        finally
                        {
                            ResetSelectedCards();
                        }
                    };
                    
                    timer.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"A apărut o eroare: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                ResetSelectedCards();
            }
        }
        
        private void ResetSelectedCards()
        {
            _firstSelectedCard = null;
            _secondSelectedCard = null;
            _isProcessingCards = false;
            
            if (CardClickCommand is RelayCommand relayCommand)
            {
                relayCommand.RaiseCanExecuteChanged();
            }
        }

        private void InitializeGameBoard()
        {
            GameBoard.Clear();
            int totalCards = BoardRows * BoardColumns;
            
            if (totalCards % 2 != 0)
            {
                MessageBox.Show("Eroare: Numărul total de cărți trebuie să fie par.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            _totalPairs = totalCards / 2;
            
            List<CardViewModel> cards = new List<CardViewModel>();
            
            for (int i = 0; i < _totalPairs; i++)
            {
                cards.Add(new CardViewModel
                {
                    Id = i * 2,
                    PairId = i,
                    FaceImagePath = $"../Images/{SelectedCategory}/card{i}.png",
                    BackImagePath = "../Images/card_back.png"
                });
                
                cards.Add(new CardViewModel
                {
                    Id = i * 2 + 1,
                    PairId = i,
                    FaceImagePath = $"../Images/{SelectedCategory}/card{i}.png",
                    BackImagePath = "../Images/card_back.png"
                });
            }
            
            Random rng = new Random();
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                CardViewModel temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
            }
            
            foreach (var card in cards)
            {
                GameBoard.Add(card);
            }
        }

        private void LoadGameBoard(List<Card> cards)
        {
            GameBoard.Clear();
            
            foreach (var card in cards)
            {
                GameBoard.Add(new CardViewModel
                {
                    Id = card.Id,
                    PairId = card.PairId,
                    FaceImagePath = card.ImagePath,
                    BackImagePath = "/Images/card_back.png",
                    IsFlipped = card.IsFlipped,
                    IsMatched = card.IsMatched
                });
            }
            
            _totalPairs = cards.Count / 2;
            
            int matchedCards = 0;
            foreach (var card in cards)
            {
                if (card.IsMatched)
                {
                    matchedCards++;
                }
            }
            _pairsFound = matchedCards / 2;
        }
    }

    public class CardViewModel : ViewModelBase
    {
        private int _id;
        private int _pairId;
        private string _faceImagePath;
        private string _backImagePath;
        private bool _isFlipped;
        private bool _isMatched;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
        
        public int PairId
        {
            get => _pairId;
            set => SetProperty(ref _pairId, value);
        }

        public string FaceImagePath
        {
            get => _faceImagePath;
            set => SetProperty(ref _faceImagePath, value);
        }

        public string BackImagePath
        {
            get => _backImagePath;
            set => SetProperty(ref _backImagePath, value);
        }

        public bool IsFlipped
        {
            get => _isFlipped;
            set
            {
                if (SetProperty(ref _isFlipped, value))
                {
                    OnPropertyChanged(nameof(CurrentImagePath));
                    OnPropertyChanged(nameof(CardOpacity));
                }
            }
        }

        public bool IsMatched
        {
            get => _isMatched;
            set
            {
                if (SetProperty(ref _isMatched, value))
                {
                    if (value)
                    {
                        IsFlipped = true;
                    }
                    OnPropertyChanged(nameof(CurrentImagePath));
                    OnPropertyChanged(nameof(CardOpacity));
                }
            }
        }

        public string CurrentImagePath => IsFlipped ? FaceImagePath : BackImagePath;
        
        public double CardOpacity => IsMatched ? 0.5 : 1.0;
    }
} 