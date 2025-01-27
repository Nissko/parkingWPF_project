using System.Windows;
using ParkingWork.ViewModels.Stats;

namespace ParkingWork.Windows.Stats
{
    public partial class ParkingIncomeStatisticsWindow : Window
    {
        public ParkingIncomeStatisticsWindow(ParkingIncomeStatisticsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}