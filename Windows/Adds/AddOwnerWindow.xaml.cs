using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ParkingWork.ViewModels.Adds;

namespace ParkingWork.Windows.Adds
{
    public partial class AddOwnerWindow : Window
    {
        public AddOwnerWindow(AddOwnerViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void TextBox_FIO_Validate(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("^[А-Яа-яЁё]+$");

            if (!regex.IsMatch(e.Text)) e.Handled = true;
        }

        private void TextBox_Address_Validate(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;

            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            var regex = new Regex(@"^[А-Яа-яЁё\s\d\-,./]*$");

            e.Handled = !regex.IsMatch(fullText);
        }

        private void TextBox_Phone_Validate(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex(@"^[0-9]+$");

            if (!regex.IsMatch(e.Text) || textBox_number.Text.Length >= 11) e.Handled = true;
        }

        private void TextBox_Brand_Validate(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex(@"^[A-Za-z]+$");

            if (!regex.IsMatch(e.Text)) e.Handled = true;
        }

        private void TextBox_Model_Validate(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex(@"^[A-Za-z0-9]+$");

            if (!regex.IsMatch(e.Text)) e.Handled = true;
        }

        private void TextBox_LicensePlate_Validate(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            var newText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            var regex = new Regex(@"^[A-Z]?\d{0,3}[A-Z]{0,2}\d{0,3}$");

            if (!regex.IsMatch(newText)) e.Handled = true;
        }
    }
}