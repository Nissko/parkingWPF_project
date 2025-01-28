using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
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
        
        private void TextBox_ParkingLot_Validate(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex(@"^[0-9]+$");

            if (!regex.IsMatch(e.Text)) e.Handled = true;
        }
    }
}