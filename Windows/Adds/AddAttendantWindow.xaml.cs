using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
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
        
        private void TextBox_FIO_Validate(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[А-Яа-яЁё]+$");

            if (!regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }
    }
}