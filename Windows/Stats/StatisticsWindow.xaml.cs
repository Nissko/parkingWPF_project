using System.Windows;
using ParkingWork.ViewModels;
using ParkingWork.ViewModels.Stats;

namespace ParkingWork.Windows.Stats
{
    public partial class StatisticsWindow : Window
    {
        public StatisticsWindow(StatisticsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}