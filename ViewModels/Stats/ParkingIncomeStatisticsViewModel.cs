using System.Windows;
using System.Windows.Input;

namespace ParkingWork.ViewModels.Stats
{
    public class ParkingIncomeStatisticsViewModel
    {
        public string ResultIncomes { get; set; }
        
        public ICommand RedirectBackCommand { get; }

        public ParkingIncomeStatisticsViewModel(string resultIncomes)
        {
            ResultIncomes = resultIncomes;
            RedirectBackCommand = new RelayCommand(RedirectBack);
        }

        private void RedirectBack(object parameter)
        {
            Application.Current.Windows[1]?.Close();
        }
    }
}