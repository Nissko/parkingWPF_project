using System.Windows;
using ParkingWork.ViewModels.Adds;

namespace ParkingWork.Windows.Adds
{
    public partial class AddReceiptWindow : Window
    {
        public AddReceiptWindow(AddReceiptViewModel receiptViewModel)
        {
            InitializeComponent();
            DataContext = receiptViewModel;
        }
    }
}