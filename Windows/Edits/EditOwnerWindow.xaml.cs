using System.Windows;
using ParkingWork.ViewModels.Edits;

namespace ParkingWork.Windows.Edits
{
    public partial class EditOwnerWindow : Window
    {
        public EditOwnerWindow(EditOwnerViewModel editOwnerViewModel)
        {
            InitializeComponent();
            DataContext = editOwnerViewModel;
        }
    }
}