using System.Windows;
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
    }
}