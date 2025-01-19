using System.Windows;
using ParkingWork.ViewModels.Edits;

namespace ParkingWork.Windows.Edits
{
    public partial class EditParkingWindow : Window
    {
        public EditParkingWindow(EditParkingViewModel addParkingViewModel)
        {
            InitializeComponent();
            DataContext = addParkingViewModel;
        }
    }
}