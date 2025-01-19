using System.Windows;
using ParkingWork.ViewModels.Adds;

namespace ParkingWork.Windows.Adds
{
    public partial class AddParkingLotWindow : Window
    {
        public AddParkingLotWindow(AddParkingLotViewModel addParkingLotViewModel)
        {
            InitializeComponent();
            DataContext = addParkingLotViewModel;
        }
    }
}