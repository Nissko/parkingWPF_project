using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using ParkingWork.Services;

namespace ParkingWork.ViewModels.Adds
{
    public class AddParkingViewModel : INotifyPropertyChanged
    {
        private string _name;
        private string _address;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand RedirectBackCommand { get; }

        private readonly ParkingService _parkingService;

        public AddParkingViewModel(ParkingService parkingService)
        {
            _parkingService = parkingService;
            SaveCommand = new RelayCommand(Save);
            RedirectBackCommand = new RelayCommand(RedirectBack);
        }

        private void Save(object parameter)
        {
            try
            {
                _parkingService.AddParking(Name, Address);
                MessageBox.Show("Стоянка успешно добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Закрытие окна
                Application.Current.Windows[1]?.Close();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void RedirectBack(object parameter)
        {
            Application.Current.Windows[1]?.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}