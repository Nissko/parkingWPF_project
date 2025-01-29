using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        
        private void TextBox_ParkingName_Validate(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[a-zA-ZА-Яа-яЁё0-9 /№'""-]+$");

            if (!regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }
        
        private void TextBox_Address_Validate(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;

            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            var regex = new Regex(@"^[А-Яа-яЁё\s\d\-,./]*$");

            e.Handled = !regex.IsMatch(fullText);
        }
        
        private void TextBox_Inn_Validate(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex(@"^[0-9]+$");

            if (!regex.IsMatch(e.Text) || textBox_inn.Text.Length >= 12) e.Handled = true;
        }
    }
}