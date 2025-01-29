using System;
using System.ComponentModel;
using System.Numerics;
using System.Windows;
using System.Windows.Input;
using ParkingWork.Exceptions;
using ParkingWork.Services;

namespace ParkingWork.ViewModels.Adds
{
    public class AddParkingViewModel : INotifyPropertyChanged
    {
        private string _name;
        private string _address;
        private string _inn;

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

        public string Inn
        {
            get => _inn;
            set
            {
                _inn = value;
                OnPropertyChanged(nameof(Inn));
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
                if (string.IsNullOrEmpty(Name.Replace(" ", "")) || string.IsNullOrEmpty(Address.Replace(" ", "")) ||
                    string.IsNullOrEmpty(Inn.Replace(" ", ""))) 
                {
                    ParkingException.ShowErrorMessage("Заполните все поля!");
                    return;
                }

                _parkingService.AddParking(Name, Address, BigInteger.Parse(Inn));
                ParkingException.ShowSuccessMessage("Стоянка успешно добавлена!");

                // Закрытие окна
                Application.Current.Windows[1]?.Close();
            }
            catch (ArgumentException ex)
            {
                ParkingException.ShowErrorMessage(ex.Message);
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