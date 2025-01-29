using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ParkingWork.Entities.Parking;
using ParkingWork.Exceptions;
using ParkingWork.Services;

namespace ParkingWork.ViewModels.Adds
{
    public class AddParkingLotViewModel : INotifyPropertyChanged
    {
        private string _name;
        private string _parkingId;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string ParkingId
        {
            get => _parkingId;
            set
            {
                _parkingId = value;
                OnPropertyChanged(nameof(ParkingId));
            }
        }

        public ObservableCollection<Parkings> Parkings { get; set; }
        public ObservableCollection<ParkingLots> ParkingLotsList { get; set; }
        public Parkings SelectedParking { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand RedirectBackCommand { get; }

        private readonly ParkingLotService _parkingLotServiceService;

        public AddParkingLotViewModel(ParkingLotService parkingLotServiceService,
            ObservableCollection<Parkings> parkingsList, ObservableCollection<ParkingLots> parkingLotsList)
        {
            _parkingLotServiceService = parkingLotServiceService;
            Parkings = parkingsList;
            ParkingLotsList = parkingLotsList;

            SaveCommand = new RelayCommand(Save);
            RedirectBackCommand = new RelayCommand(RedirectBack);
        }

        private void Save(object parameter)
        {
            try
            {
                var validate = ValidateParkingLot();
                
                if (!validate)
                    return;
                
                Name = $"Место №{_name}";

                _parkingLotServiceService.AddParkingLot(Name, SelectedParking.Id.ToString(), SelectedParking.Name);
                ParkingException.ShowSuccessMessage("Парковочное место успешно добавлено!");

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

        public bool ValidateParkingLot()
        {
            if (SelectedParking == null)
            {
                ParkingException.ShowErrorMessage("Пожалуйста, выберите стоянку.");
                return false;
            }

            if (string.IsNullOrEmpty(Name.Replace(" ", "")) || SelectedParking.Id == Guid.Empty ||
                string.IsNullOrEmpty(SelectedParking.Name))
            {
                ParkingException.ShowErrorMessage("Заполните все поля!");
                return false;
            }

            if (Name.Replace(" ", "").Length <= 0)
            {
                ParkingException.ShowErrorMessage("Номер парковки пуст, введите значение!");
                return false;
            }

            var parkingLot = ParkingLotsList.Where(t=> t.ParkingId == SelectedParking.Id).ToList();

            if (parkingLot.Any(pl => pl.Name.Replace("Место №", "") == Name))
            {
                ParkingException.ShowErrorMessage("Введенный номер парковки уже существует, введите другой номер!");
                return false;
            }

            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}