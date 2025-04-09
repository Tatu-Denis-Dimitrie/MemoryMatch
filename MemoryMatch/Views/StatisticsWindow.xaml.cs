using System.Windows;
using MemoryMatch.ViewModels;

namespace MemoryMatch.Views
{
    public partial class StatisticsWindow : Window
    {
        private readonly StatisticsViewModel _viewModel;

        public StatisticsWindow()
        {
            InitializeComponent();
            _viewModel = new StatisticsViewModel();
            DataContext = _viewModel;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            _viewModel.LoadStatistics();
        }
    }
} 