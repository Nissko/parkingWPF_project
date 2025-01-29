using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
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
        
        private void TextBox_ParkingLot_Validate(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex(@"^[0-9]+$");

            if (!regex.IsMatch(e.Text)) e.Handled = true;
        }
    }
}