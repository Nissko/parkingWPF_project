using System.Windows;
using ParkingWork.ViewModels.Adds;

namespace ParkingWork.Windows.Adds
{
    public partial class AddAttendantWindow : Window
    {
        public AddAttendantWindow(AddAttendantViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}