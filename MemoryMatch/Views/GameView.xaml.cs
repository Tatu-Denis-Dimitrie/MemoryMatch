using System.Windows;
using MemoryMatch.ViewModels;

namespace MemoryMatch.Views
{
    public partial class GameView : Window
    {
        public GameView()
        {
            InitializeComponent();
        }

        public GameView(string username) : this()
        {
            if (DataContext is GameViewModel viewModel)
            {
                viewModel.CurrentUsername = username;
            }
        }
    }
} 