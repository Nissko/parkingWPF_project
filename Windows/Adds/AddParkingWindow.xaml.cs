using System.Windows;
using ParkingWork.ViewModels.Adds;

namespace ParkingWork.Windows.Adds
{
    public partial class AddParkingWindow : Window
    {
        public AddParkingWindow(AddParkingViewModel addParkingViewModel)
        {
            InitializeComponent();
            DataContext = addParkingViewModel;
        }
    }
}