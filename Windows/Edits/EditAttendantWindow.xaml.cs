using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
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