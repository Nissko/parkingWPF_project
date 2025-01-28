using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using ParkingWork.Entities.Vehicle;
using ParkingWork.Entities.Vehicle.Enum;
using ParkingWork.Exceptions;
using ParkingWork.Services;

namespace ParkingWork.ViewModels.Adds
{
    public class AddOwnerViewModel : INotifyPropertyChanged
    {
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        public ObservableCollection<Vehicles> Cars { get; set; } = new ObservableCollection<Vehicles>();

        private string _brand;

        public string Brand
        {
            get => _brand;
            set
            {
                _brand = value;
                OnPropertyChanged(nameof(Brand));
            }
        }

        private string _model;

        public string Model
        {
            get => _model;
            set
            {
                _model = value;
                OnPropertyChanged(nameof(Model));
            }
        }

        private string _licensePlate;

        public string LicensePlate
        {
            get => _licensePlate;
            set
            {
                _licensePlate = value;
                OnPropertyChanged(nameof(LicensePlate));
            }
        }

        private VehicleColorEnums _selectedColor;

        public VehicleColorEnums SelectedColor
        {
            get => _selectedColor;
            set
            {
                _selectedColor = value;
                OnPropertyChanged(nameof(SelectedColor));
            }
        }

        private readonly Guid _newClientId = Guid.NewGuid();

        public ICommand AddCarCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand RedirectBackCommand { get; }

        private readonly OwnerService _ownerService;

        public AddOwnerViewModel(OwnerService ownerService)
        {
            _ownerService = ownerService;

            AddCarCommand = new RelayCommand(AddCar);
            SaveCommand = new RelayCommand(Save);
            RedirectBackCommand = new RelayCommand(RedirectBack);
        }

        private void AddCar(object parameter)
        {
            if (string.IsNullOrEmpty(LicensePlate) || string.IsNullOrEmpty(Brand) || string.IsNullOrEmpty(Model) ||
                string.IsNullOrEmpty(_selectedColor.ToString()))
            {
                ParkingException.ShowErrorMessage("Невозможно добавить авто! Заполните все поля!");
                return;
            }

            Cars.Add(new Vehicles(Guid.NewGuid(), _newClientId, LicensePlate, Brand, Model, _selectedColor));

            // Очистка формы
            Brand = string.Empty;
            Model = string.Empty;
            LicensePlate = string.Empty;
            SelectedColor = VehicleColorEnums.Red;
        }

        private void Save(object parameter)
        {
            try
            {
                if (_newClientId == Guid.Empty || string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Surname) ||
                    string.IsNullOrEmpty(Patronymic) || string.IsNullOrEmpty(Address) || string.IsNullOrEmpty(Phone) ||
                    Cars.Count == 0)
                {
                    ParkingException.ShowErrorMessage("Заполните все поля!");
                    return;
                }

                _ownerService.AddOwner(_newClientId, Name, Surname, Patronymic, Address, Phone, Cars);
                ParkingException.ShowSuccessMessage("Клиент успешно добавлен!");

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