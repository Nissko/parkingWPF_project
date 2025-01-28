using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ParkingWork.Entities.Attendants;
using ParkingWork.Entities.Owner;
using ParkingWork.Entities.Parking;
using ParkingWork.Entities.Parking.Receipt;
using ParkingWork.Entities.Vehicle;
using ParkingWork.Exceptions;
using ParkingWork.Services;
using ParkingWork.Windows.Adds;

namespace ParkingWork.ViewModels.Adds
{
    public class AddReceiptViewModel : INotifyPropertyChanged
    {
        private readonly List<Receipts> _receiptsListToAdd = new List<Receipts>();

        /// <summary>
        ///     Серии квитанций
        /// </summary>
        private const string _seriesDatas = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";


        private decimal _price;
        private int _days;
        private Parkings _selectedParking;
        private ParkingLots _selectedParkingLot;
        private Owners _selectedOwner;
        private Vehicles _selectedVehicle;

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
        ///     Полный список парковок
        /// </summary>
        public ObservableCollection<ParkingLots> ParkingLots { get; set; }

        /// <summary>
        ///     Список свободных парковок
        /// </summary>
        public ObservableCollection<ParkingLots> AvailableParkingLots { get; set; }

        public ObservableCollection<Owners> Owners { get; set; }

        /// <summary>
        ///     Список автомобилей клиента
        /// </summary>
        public ObservableCollection<Vehicles> AvailableOwnerCars { get; set; }

        public ObservableCollection<Attendants> Attendants { get; set; }
        public ObservableCollection<Vehicles> Vehicles { get; set; }

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

        public Owners SelectedOwner
        {
            get => _selectedOwner;
            set
            {
                _selectedOwner = value;
                OnPropertyChanged(nameof(SelectedOwner));
                LoadAvailableOwnerCars();
            }
        }

        public Vehicles SelectedVehicle
        {
            get => _selectedVehicle;
            set
            {
                _selectedVehicle = value;
                OnPropertyChanged(nameof(SelectedVehicle));
            }
        }

        public Attendants SelectedAttendant { get; set; }

        public ICommand GenerateReceiptCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand RedirectBackCommand { get; }

        /*Сервис*/
        private readonly ReceiptService _receiptService;

        public AddReceiptViewModel(ReceiptService receiptService, ObservableCollection<Receipts> receiptsList,
            ObservableCollection<Parkings> parkingsList, ObservableCollection<ParkingLots> parkingLotsList,
            ObservableCollection<Owners> ownersList, ObservableCollection<Attendants> attendantsList,
            ObservableCollection<Vehicles> vehiclesList)
        {
            _receiptService = receiptService;
            Receipts = receiptsList;
            Parkings = parkingsList;
            ParkingLots = parkingLotsList;
            AvailableParkingLots = new ObservableCollection<ParkingLots>(); // Пустой список по умолчанию
            Owners = ownersList;
            AvailableOwnerCars = new ObservableCollection<Vehicles>();
            Attendants = attendantsList;
            Vehicles = vehiclesList;

            GenerateReceiptCommand = new RelayCommand(GenerateReceipt);
            SaveCommand = new RelayCommand(Save);
            RedirectBackCommand = new RelayCommand(RedirectBack);
        }

        /// <summary>
        ///     Метод для поиска свободных мест
        /// </summary>
        private void LoadAvailableParkingLots()
        {
            AvailableParkingLots.Clear();

            if (SelectedParking != null)
                // Фильтруем места, которые относятся к выбранной стоянке и свободны
                foreach (var lot in ParkingLots)
                    if (lot.ParkingId == SelectedParking.Id && lot.IsFree)
                        AvailableParkingLots.Add(lot);
        }

        private void LoadAvailableOwnerCars()
        {
            AvailableOwnerCars.Clear();

            if (SelectedOwner != null)
                // Фильтруем машны, которые относятся к выбранному владельцу
                foreach (var car in Vehicles)
                    if (car.ClientId == SelectedOwner.Id)
                        AvailableOwnerCars.Add(car);
        }

        private async void GenerateReceipt(object parameter)
        {
            try
            {
                _receiptsListToAdd.Clear();

                if (SelectedParking == null || SelectedParkingLot == null || SelectedOwner == null ||
                    SelectedAttendant == null || SelectedVehicle == null || string.IsNullOrEmpty(Days.ToString()) ||
                    Price <= 0)
                {
                    ParkingException.ShowErrorMessage("Пожалуйста, заполните все поля.");
                    return;
                }

                var seriesReceipt = await GenerateSeries(Receipts, SelectedParking);
                var numberReceipt = await GenerateNumber(Receipts, SelectedParking, seriesReceipt);

                var preloadReceipt = new Receipts(seriesReceipt, numberReceipt, SelectedOwner, SelectedParking,
                    SelectedParkingLot,
                    SelectedAttendant, Days, Price, SelectedVehicle.Id);

                _receiptsListToAdd.Add(preloadReceipt);

                #region Теги для заполнения

                var tags = new Dictionary<string, string>
                {
                    { "<PARKINGNAME>", preloadReceipt.Parking.Name },
                    { "<INNPARKING>", $"ИНН: {preloadReceipt.Parking.Inn}" },
                    { "<ADDRESSPARKING>", preloadReceipt.Parking.Address },
                    { "<SERIES>", preloadReceipt.Series },
                    { "<NUMBER>", preloadReceipt.Number },
                    { "<PARKINGLOT>", preloadReceipt.ParkingLot.Name },

                    {
                        "<CARBRAND>",
                        preloadReceipt.Owner.Vehicles.FirstOrDefault(t => t.Id == preloadReceipt.SelectedCarId)?.Brand
                    },
                    {
                        "<CARMODEL>",
                        preloadReceipt.Owner.Vehicles.FirstOrDefault(t => t.Id == preloadReceipt.SelectedCarId)?.Model
                    },
                    {
                        "<LICENSEPLATE>",
                        preloadReceipt.Owner.Vehicles.FirstOrDefault(t => t.Id == preloadReceipt.SelectedCarId)
                            ?.LicensePlate
                    },

                    { "<FIOOWNER>", preloadReceipt.Owner.FullNameInLine },
                    { "<ADDRESSOWNER>", preloadReceipt.Owner.Address },
                    { "<PHONEOWNER>", preloadReceipt.Owner.Phone },

                    { "<STARTDATE>", preloadReceipt.GetStartDate().ToShortDateString() },
                    { "<SHOUR>", preloadReceipt.GetStartDate().ToString("HH") },
                    { "<SMINE>", preloadReceipt.GetStartDate().ToString("mm") },
                    { "<STARTPARK>", preloadReceipt.GetStartDate().ToString("dd.MM.yyyy") },
                    { "<ENDPARK>", preloadReceipt.EndDate.ToString("dd.MM.yyyy") },

                    { "<FIOATTENDANT>", preloadReceipt.Attendants.FullNameInLine },

                    { "<DAYS>", preloadReceipt.Days.ToString() },
                    { "<AMOUNT>", preloadReceipt.Amount.ToString() }
                };

                #endregion

                var wordService = new WordService();
                var outputWordPath = wordService.GenerateReceipt(tags);
                var outputPath = wordService.ConvertWordToPdf(outputWordPath);

                var currentWindow = Application.Current.Windows[1] as AddReceiptWindow;
                currentWindow?.ShowPdfInWebBrowser(outputPath);
            }
            catch (ArgumentException ex)
            {
                ParkingException.ShowErrorMessage(ex.Message);
                throw;
            }
        }

        private void Save(object parameter)
        {
            try
            {
                var newReceiptParking = _receiptsListToAdd.FirstOrDefault();
                if (newReceiptParking is null)
                {
                    ParkingException.ShowErrorMessage("Не была найдена сформированная квитанция!");
                    return;
                }

                if (newReceiptParking.Parking == null || newReceiptParking.ParkingLot == null ||
                    newReceiptParking.Owner == null || newReceiptParking.Attendants == null ||
                    newReceiptParking.Owner.Vehicles == null ||
                    string.IsNullOrEmpty(newReceiptParking.Days.ToString()) || newReceiptParking.Price <= 0)
                {
                    ParkingException.ShowErrorMessage("Нет всех данных в квитанции.");
                    return;
                }

                _receiptService.AddReceipt(newReceiptParking.Series, newReceiptParking.Number, newReceiptParking.Owner,
                    newReceiptParking.Parking, newReceiptParking.ParkingLot, newReceiptParking.Attendants,
                    newReceiptParking.Days, newReceiptParking.Price, newReceiptParking.SelectedCarId,
                    newReceiptParking.GetStartDate());

                ParkingException.ShowSuccessMessage("Квитанция успешно добавлена");

                Application.Current.Windows[1]?.Close();
            }
            catch (Exception ex)
            {
                ParkingException.ShowErrorMessage(ex.Message);
                throw;
            }
        }

        #region Генерация серии и номера

        private async Task<string> GenerateSeries(ObservableCollection<Receipts> receipts, Parkings selectedParking)
        {
            var receiptFromSelectedParking = receipts.Where(t => t.Parking.Id == selectedParking.Id);

            if (!receiptFromSelectedParking.Any()) return "A";

            var maxNumberBySeries = receiptFromSelectedParking
                .GroupBy(r => r.Series)
                .Select(group => new
                {
                    Series = group.Key,
                    MaxNumber = group.Max(r => int.Parse(r.Number))
                })
                .OrderBy(g => g.Series)
                .ToList();

            var lastSeriesInfo = maxNumberBySeries.LastOrDefault();

            if (lastSeriesInfo != null && lastSeriesInfo.MaxNumber >= 999999)
            {
                var currentSeriesIndex = _seriesDatas.IndexOf(lastSeriesInfo.Series);

                if (currentSeriesIndex == _seriesDatas.Length - 1)
                    throw new InvalidOperationException(
                        "Превышен лимит серий. Добавьте больше символов в алфавит серий.");

                return _seriesDatas[currentSeriesIndex + 1].ToString();
            }

            return lastSeriesInfo?.Series ?? "A";
        }

        private async Task<string> GenerateNumber(ObservableCollection<Receipts> receipts, Parkings selectedParking,
            string seriesReceipt)
        {
            return await Task.Run(() =>
            {
                var receiptsForSeries = receipts
                    .Where(r => r.Parking.Id == selectedParking.Id && r.Series == seriesReceipt)
                    .ToList();

                if (!receiptsForSeries.Any()) return "000001";

                var maxNumber = receiptsForSeries
                    .Select(r => int.Parse(r.Number))
                    .Max();

                if (maxNumber < 999999) return (maxNumber + 1).ToString("D6");
                return "000001";
            });
        }

        #endregion

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