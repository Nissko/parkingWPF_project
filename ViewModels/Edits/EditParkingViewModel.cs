using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ParkingWork.Entities.Parking;
using ParkingWork.Services;

namespace ParkingWork.ViewModels.Edits
{
    public class EditParkingViewModel : INotifyPropertyChanged
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
            
            ChangeCommand = new RelayCommand(Change);
            RedirectBackCommand = new RelayCommand(RedirectBack);
        }

        private void Change(object parameter)
        {
            try
            {
                if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Address))
                    throw new ArgumentNullException("Поля не могут быть пустыми");

                Parkings.FirstOrDefault(t => t.Id == _parkingChange.Id)?.ChangeParking(Name, Address);

                _parkingService.EditParking(Parkings, _parkingChange.Id);

                MessageBox.Show("Стоянка успешно изменена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

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