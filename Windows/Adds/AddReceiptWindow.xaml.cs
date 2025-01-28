using System;
using System.IO;
using System.Windows;
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
    }
}