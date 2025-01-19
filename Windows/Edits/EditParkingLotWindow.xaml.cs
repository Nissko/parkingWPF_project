using System.Windows;
using ParkingWork.ViewModels.Edits;

namespace ParkingWork.Windows.Edits
{
    public partial class EditParkingLotWindow : Window
    {
        public EditParkingLotWindow(EditParkingLotViewModel editParkingLotViewModel)
        {
            InitializeComponent();
            DataContext = editParkingLotViewModel;
        }
    }
}