using System.Windows;
using ParkingWork.ViewModels;

namespace ParkingWork
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ParkingViewModel();
        }
    }
}
