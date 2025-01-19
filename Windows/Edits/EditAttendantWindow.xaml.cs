using System.Windows;
using ParkingWork.ViewModels.Edits;

namespace ParkingWork.Windows.Edits
{
    public partial class EditAttendantWindow : Window
    {
        public EditAttendantWindow(EditAttendantViewModel editAttendantViewModel)
        {
            InitializeComponent();
            DataContext = editAttendantViewModel;
        }
    }
}