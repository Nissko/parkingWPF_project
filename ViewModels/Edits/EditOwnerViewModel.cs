using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ParkingWork.Entities.Owner;
using ParkingWork.Entities.Vehicle;
using ParkingWork.Entities.Vehicle.Enum;
using ParkingWork.Exceptions;
using ParkingWork.Services;

namespace ParkingWork.ViewModels.Edits
{
    public class EditOwnerViewModel : INotifyPropertyChanged
    {
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        public ObservableCollection<Vehicles> Cars { get; set; } = new ObservableCollection<Vehicles>();

        private Vehicles _selectedCar;

        public Vehicles SelectedCar
        {
            get => _selectedCar;
            set
            {
                _selectedCar = value;
                if (_selectedCar != null)
                {
                    CarId = _selectedCar.Id.ToString();
                    Brand = _selectedCar.Brand;
                    Model = _selectedCar.Model;
                    LicensePlate = _selectedCar.LicensePlate;
                    SelectedColor = _selectedCar.Color;
                    SelectedColorString = SelectedColor.ToString();
                }

                OnPropertyChanged(nameof(SelectedCar));
            }
        }

        private string _carId;

        public string CarId
        {
            get => _carId;
            set
            {
                _carId = value;
                OnPropertyChanged(nameof(CarId));
            }
        }

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
        
        private string _selectedColorString;

        public string SelectedColorString
        {
            get => _selectedColor.ToString();
            set
            {
                if (_selectedColorString != value)
                {
                    _selectedColorString = value;
                    SelectedColor = (VehicleColorEnums)Enum.Parse(typeof(VehicleColorEnums), _selectedColorString);
                    OnPropertyChanged(nameof(SelectedColorString));
                }
            }
        }

        public ICommand ChangeCommand { get; }
        public ICommand RedirectBackCommand { get; }
        public ICommand SaveCarCommand { get; }

        public ObservableCollection<Owners> OwnersList { get; set; }
        private readonly Owners _ownerChange;
        private readonly OwnerService _ownerService;

        public EditOwnerViewModel(Owners ownerChange, OwnerService ownerService,
            ObservableCollection<Owners> owners)
        {
            _ownerChange = ownerChange;
            _ownerService = ownerService;
            OwnersList = owners;

            Surname = ownerChange.Surname;
            Name = ownerChange.Name;
            Patronymic = ownerChange.Patronymic;
            Address = ownerChange.Address;
            Phone = ownerChange.Phone;

            foreach (var vehicle in ownerChange.Vehicles) Cars.Add(vehicle);

            ChangeCommand = new RelayCommand(Change);
            RedirectBackCommand = new RelayCommand(RedirectBack);
            SaveCarCommand = new RelayCommand(SaveCarChanges);
        }

        private void SaveCarChanges(object parameter)
        {
            if (string.IsNullOrEmpty(CarId))
            {
                ParkingException.ShowErrorMessage("Не выбрана машина");
                return;
            }
            
            var carSelected = Cars.FirstOrDefault(x => x.Id == Guid.Parse(CarId));

            if (carSelected != null)
            {
                carSelected.ChangeVehicle(LicensePlate, Brand, Model, SelectedColor);

                var index = Cars.IndexOf(SelectedCar);
                if (index >= 0)
                {
                    Cars[index] = null;
                    Cars[index] = carSelected;
                }

                OnPropertyChanged(nameof(Cars));
            }
        }

        private void Change(object parameter)
        {
            try
            {
                if (string.IsNullOrEmpty(Surname) || string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Patronymic) ||
                    string.IsNullOrEmpty(Address) || string.IsNullOrEmpty(Phone) || Cars.Count == 0)
                    throw new Exception("Поля не могут быть пустыми");

                OwnersList.FirstOrDefault(t => t.Id == _ownerChange.Id)
                    ?.ChangeOwner(Name, Surname, Patronymic, Address, Phone, Cars);

                _ownerService.ChangeOwner(OwnersList, _ownerChange.Id);

                ParkingException.ShowSuccessMessage("Изменения успешно сохранены!");

                // Закрытие окна
                Application.Current.Windows[1]?.Close();
            }
            catch (Exception ex)
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