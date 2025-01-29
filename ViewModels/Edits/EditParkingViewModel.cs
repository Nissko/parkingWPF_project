using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Input;
using ParkingWork.Entities.Parking;
using ParkingWork.Exceptions;
using ParkingWork.Services;

namespace ParkingWork.ViewModels.Edits
{
    public class EditParkingViewModel : INotifyPropertyChanged
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

        public ICommand ChangeCommand { get; }
        public ICommand RedirectBackCommand { get; }

        // Parcing collection
        public ObservableCollection<Parkings> Parkings { get; set; }
        // Changing parking
        private readonly Parkings _parkingChange;
        // Parking sevice
        private readonly ParkingService _parkingService;

        public EditParkingViewModel(Parkings parkingToChange, ParkingService parkingService, ObservableCollection<Parkings> parkings)
        {
            _parkingChange = parkingToChange;
            _parkingService = parkingService;
            Parkings = parkings;
            
            Name = _parkingChange.Name;
            Address = _parkingChange.Address;
            Inn = _parkingChange.Inn.ToString();
            
            ChangeCommand = new RelayCommand(Change);
            RedirectBackCommand = new RelayCommand(RedirectBack);
        }

        private void Change(object parameter)
        {
            try
            {
                if (string.IsNullOrEmpty(Name.Replace(" ", "")) || string.IsNullOrEmpty(Address.Replace(" ", "")) ||
                    string.IsNullOrEmpty(Inn.ToString().Replace(" ", "")))
                {
                    ParkingException.ShowErrorMessage("Поля не могут быть пустыми");
                    return;
                }

                Parkings.FirstOrDefault(t => t.Id == _parkingChange.Id)
                    ?.ChangeParking(Name, Address, BigInteger.Parse(Inn));

                _parkingService.EditParking(Parkings, _parkingChange.Id);

                ParkingException.ShowSuccessMessage("Стоянка успешно изменена!");
                    
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