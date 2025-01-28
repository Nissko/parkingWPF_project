using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ParkingWork.Exceptions;
using ParkingWork.ViewModels.Adds;

namespace ParkingWork.Windows.Adds
{
    public partial class AddReceiptWindow : Window
    {
        public AddReceiptWindow(AddReceiptViewModel receiptViewModel)
        {
            InitializeComponent();
            DataContext = receiptViewModel;
            SaveButton.Visibility = Visibility.Collapsed;
        }
        
        public async void ShowPdfInWebBrowser(string pdfFilePath)
        {
            if (!File.Exists(pdfFilePath))
            {
                ParkingException.ShowErrorMessage("Файл PDF не найден!");
                return;
            }
            
            await WordViewer.EnsureCoreWebView2Async();

            WordViewer.CoreWebView2.Navigate("about:blank");
            WordViewer.CoreWebView2.Navigate(new Uri(pdfFilePath).AbsoluteUri);
            SaveButton.Visibility = Visibility.Visible;
        }
        
        private void TextBox_Price_Validate(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex(@"^[0-9,]+$");

            TextBox textBox = sender as TextBox;

            if (!regex.IsMatch(e.Text))
            {
                e.Handled = true;
                return;
            }

            if (textBox.Text.Contains(",") || textBox.Text.Contains("."))
            {
                if (e.Text == "," || e.Text == ".")
                {
                    e.Handled = true;
                    return;
                }

                var parts = textBox.Text.Split(new char[] { ',', '.' });
                if (parts.Length > 1 && parts[1].Length >= 2)
                {
                    e.Handled = true;
                    return;
                }
            }

            if (textBox.Text.Length >= 15)
            {
                e.Handled = true;
            }
        }
    }
}