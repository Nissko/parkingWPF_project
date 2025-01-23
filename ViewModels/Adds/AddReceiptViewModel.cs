using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using ParkingWork.Entities.Attendants;
using ParkingWork.Entities.Owner;
using ParkingWork.Entities.Parking;
using ParkingWork.Entities.Parking.Receipt;
using ParkingWork.Services;

namespace ParkingWork.ViewModels.Adds
{
    public class AddReceiptViewModel : INotifyPropertyChanged
    {
        private decimal _price;
        private int _days;
        private Parkings _selectedParking;
        private ParkingLots _selectedParkingLot;

        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
            }
        }

        public int Days
        {
            get => _days;
            set
            {
                _days = value;
                OnPropertyChanged(nameof(Days));
            }
        }

        /*Коллекции*/
        public ObservableCollection<Receipts> Receipts { get; set; }
        public ObservableCollection<Parkings> Parkings { get; set; }
        /// <summary>
        /// Полный список парковок
        /// </summary>
        public ObservableCollection<ParkingLots> ParkingLots { get; set; }
        /// <summary>
        /// Список свободных парковок
        /// </summary>
        public ObservableCollection<ParkingLots> AvailableParkingLots { get; set; }
        public ObservableCollection<Owners> Owners { get; set; }
        public ObservableCollection<Attendants> Attendants { get; set; }

        /*Выбранные значения*/
        public Parkings SelectedParking
        {
            get => _selectedParking;
            set
            {
                _selectedParking = value;
                OnPropertyChanged(nameof(SelectedParking));
                LoadAvailableParkingLots();
            }
        }

        public ParkingLots SelectedParkingLot
        {
            get => _selectedParkingLot;
            set
            {
                _selectedParkingLot = value;
                OnPropertyChanged(nameof(SelectedParkingLot));
            }
        }

        public Owners SelectedOwner { get; set; }
        public Attendants SelectedAttendant { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand RedirectBackCommand { get; }

        /*Сервис*/
        private readonly ReceiptService _receiptService;

        public AddReceiptViewModel(ReceiptService receiptService, ObservableCollection<Receipts> receiptsList,
            ObservableCollection<Parkings> parkingsList, ObservableCollection<ParkingLots> parkingLotsList,
            ObservableCollection<Owners> ownersList, ObservableCollection<Attendants> attendantsList)
        {
            _receiptService = receiptService;
            Receipts = receiptsList;
            Parkings = parkingsList;
            ParkingLots = parkingLotsList;
            AvailableParkingLots = new ObservableCollection<ParkingLots>(); // Пустой список по умолчанию
            Owners = ownersList;
            Attendants = attendantsList;

            SaveCommand = new RelayCommand(Save);
            RedirectBackCommand = new RelayCommand(RedirectBack);
        }

        /// <summary>
        /// Метод для поиска свободных мест
        /// </summary>
        private void LoadAvailableParkingLots()
        {
            AvailableParkingLots.Clear();

            if (SelectedParking != null)
            {
                // Фильтруем места, которые относятся к выбранной стоянке и свободны
                foreach (var lot in ParkingLots)
                {
                    if (lot.ParkingId == SelectedParking.Id && lot.IsFree)
                    {
                        AvailableParkingLots.Add(lot);
                    }
                }
            }
        }

        private void Save(object parameter)
        {
            try
            {
                if (SelectedParking == null || SelectedParkingLot == null || SelectedOwner == null || SelectedAttendant == null)
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.", "Указаны не все данные!");
                    return;
                }

                _receiptService.AddReceipt(
                    series: "1",
                    number: "000001",
                    owner: SelectedOwner,
                    parking: SelectedParking,
                    parkingLot: SelectedParkingLot,
                    attendant: SelectedAttendant,
                    days: Days,
                    price: Price
                );

                MessageBox.Show("Квитанция успешно создана!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

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
