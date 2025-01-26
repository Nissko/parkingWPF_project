using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using ParkingWork.Entities.Attendants;
using ParkingWork.Entities.Owner;
using ParkingWork.Entities.Parking;
using ParkingWork.Entities.Parking.Receipt;
using ParkingWork.Entities.Vehicle;
using ParkingWork.Services;
using ParkingWork.ViewModels.Adds;
using ParkingWork.ViewModels.Edits;
using ParkingWork.ViewModels.Stats;
using ParkingWork.Windows.Adds;
using ParkingWork.Windows.Edits;
using ParkingWork.Windows.Stats;

namespace ParkingWork.ViewModels
{
    /// <summary>
    /// Главная модель парковки
    /// </summary>
    public class ParkingViewModel : INotifyPropertyChanged
    {
        private string _filePath = $"C:\\GitHubRepositories\\parkingWPF_project\\DbFolder\\ParkingDB.xlsx";

        #region Коллекции
        
        public ObservableCollection<Parkings> ParkingsCompany { get; set; }
        public ObservableCollection<ParkingLots> ParkingLots { get; set; }
        public ObservableCollection<Owners> Owners { get; set; }
        public ObservableCollection<Attendants> Attendants { get; set; }
        public ObservableCollection<Receipts> Receipts { get; set; }
        public ObservableCollection<Vehicles> Vehicles { get; set; }
        
        #endregion

        #region Команды

        #region Команды на добавление

        public ICommand AddOwnerCommand { get; }
        public ICommand AddAttendantCommand { get; }
        public ICommand AddParkingCompanyCommand { get; }
        public ICommand AddParkingLotCommand { get; }
        public ICommand AddReceiptCommand { get; }

        #endregion

        #region Команды на изменение

        public ICommand EditParkingCompanyCommand { get; }
        public ICommand EditParkingLotCommand { get; }
        public ICommand EditAttendantCommand { get; }
        public ICommand EditOwnerCommand { get; }

        #endregion

        #region Команды на сохранение/статистику

        public ICommand SaveToExcelCommand { get; }
        public ICommand SaveToTextFileCommand { get; }
        public ICommand PrintReceiptCommand { get; }
        public ICommand GetReceiptStatsCommand { get; }

        #endregion
        
        public ICommand CloseApplicationCommand { get; }

        #endregion

        #region Сервисы

        private readonly AttendantService _attendantService;
        private readonly ParkingService _parkingService;
        private readonly ParkingLotService _parkingLotService;
        private readonly OwnerService _ownerService;
        private readonly ExcelDataLoaderService _excelDataLoaderService;
        private readonly ExcelDataSaverService _excelDataSaverService;
        private readonly ReceiptService _receiptService;

        #endregion
        
        public ParkingViewModel()
        {
            #region Инициализация коллекций

            ParkingsCompany = new ObservableCollection<Parkings>();
            ParkingLots = new ObservableCollection<ParkingLots>();
            Owners = new ObservableCollection<Owners>();
            Attendants = new ObservableCollection<Attendants>();
            Receipts = new ObservableCollection<Receipts>();
            Vehicles = new ObservableCollection<Vehicles>();

            #endregion

            #region Инициализация сервисов

            _attendantService = new AttendantService();
            _parkingService = new ParkingService();
            _parkingLotService = new ParkingLotService();
            _ownerService = new OwnerService();
            //TODO: добавить _filePath
            _excelDataLoaderService = new ExcelDataLoaderService();
            _excelDataSaverService = new ExcelDataSaverService(_filePath);
            _receiptService = new ReceiptService();

            #endregion

            #region Подписание на события добавления

            _attendantService.AttendantAdded += attendant => OnAttendantAdded(attendant);;
            _parkingService.ParkingAdded += parking => OnParkingAdded(parking);
            _parkingLotService.ParkingLotAdded += parkingLot => OnParkingLotAdded(parkingLot);
            _ownerService.OwnerAdded += owner => OnOwnerAdded(owner);
            _receiptService.ReceiptAdded += receipt => OnReceiptAdded(receipt);

            #endregion

            #region Подписание на события изменения

            // TODO: предусмотреть изменения в других коллекциях (проверить)
            _parkingService.ParkingChanged += parking =>
            {
                if (parking == null) return;
                var updatedList = new ObservableCollection<Parkings>(ParkingsCompany);

                var existingParking = updatedList.FirstOrDefault(p => p.Id == parking.Id);
                if (existingParking != null)
                    existingParking.ChangeParking(parking.Name, parking.Address, parking.Inn);
                else
                    updatedList.Add(parking);

                ParkingsCompany = updatedList;
                OnPropertyChanged(nameof(ParkingsCompany));

                //Обновление коллекции с парковочными местами
                var updatedParkingLotsList = new ObservableCollection<ParkingLots>(ParkingLots.Select(parkingLot =>
                {
                    if (parkingLot.ParkingId == parking.Id) parkingLot.ChangeParkingName(parking.Name);
                    return parkingLot;
                }));

                ParkingLots = updatedParkingLotsList;
                OnPropertyChanged(nameof(ParkingLots));
            };
            _attendantService.AttendantChanged += attendant =>
            {
                if (attendant == null) return;
                var updatedList = new ObservableCollection<Attendants>(Attendants);

                var existingAttendant = updatedList.FirstOrDefault(p => p.Id == attendant.Id);
                if (existingAttendant != null)
                    existingAttendant.ChangeAttendant(attendant.Name, attendant.Surname, attendant.Patronymic);
                else
                    updatedList.Add(attendant);

                Attendants = updatedList;
                OnPropertyChanged(nameof(Attendants));
            };
            _parkingLotService.ParkingLotChanged += parking =>
            {
                if (parking == null) return;
                var updatedList = new ObservableCollection<ParkingLots>(ParkingLots);

                var existingParkingLot = updatedList.FirstOrDefault(p => p.Id == parking.Id);
                if (existingParkingLot != null)
                    existingParkingLot.ChangeParkingLot(parking.Name, parking.IsFree);
                else
                    updatedList.Add(parking);

                ParkingLots = updatedList;
                OnPropertyChanged(nameof(ParkingLots));

                //Обновление коллекции с парковочными местами
                var updatedParkingLotsList = new ObservableCollection<ParkingLots>(ParkingLots.Select(parkingLot =>
                {
                    if (parkingLot.ParkingId == parking.Id) parkingLot.ChangeParkingName(parking.Name);
                    return parkingLot;
                }));

                ParkingLots = updatedParkingLotsList;
                OnPropertyChanged(nameof(ParkingLots));
            };
            _ownerService.OwnerChanged += owner =>
            {
                if (owner == null) return;

                var updatedList = new ObservableCollection<Owners>(Owners);

                var existingOwner = updatedList.FirstOrDefault(o => o.Id == owner.Id);
                if (existingOwner != null)
                {
                    // Обновление данных существующего владельца
                    existingOwner.ChangeOwner(owner.Name, owner.Surname, owner.Patronymic, owner.Address, owner.Phone,
                        owner.Vehicles);

                    // Обновление автомобилей владельца (Owners)
                    foreach (var newVehicle in owner.Vehicles)
                    {
                        var existingVehicle = existingOwner.Vehicles.FirstOrDefault(v => v.Id == newVehicle.Id);
                        if (existingVehicle != null)
                            existingVehicle.ChangeVehicle(newVehicle.LicensePlate, newVehicle.Brand, newVehicle.Model,
                                newVehicle.Color);
                        else
                            existingOwner.Vehicles.Append(newVehicle);
                    }
                    
                    //Обновление коллекции с авто (Vehicles)
                    var updatedVehiclesList = new ObservableCollection<Vehicles>(Vehicles.Select(vehicle =>
                    {
                        var existingVehicle = existingOwner.Vehicles.FirstOrDefault(v => v.Id == vehicle.Id);

                        if (existingVehicle != null)
                            vehicle.ChangeVehicle(existingVehicle.LicensePlate, existingVehicle.Brand, existingVehicle.Model,
                                existingVehicle.Color);
                        
                        return vehicle;
                    }));

                    Vehicles = updatedVehiclesList;
                }
                else
                {
                    updatedList.Add(owner);
                }

                Owners = updatedList;

                OnPropertyChanged(nameof(Owners));
            };

            #endregion

            #region Инициализация команд добавления

            AddOwnerCommand = new RelayCommand(OpenAddOwnerWindow);
            AddAttendantCommand = new RelayCommand(OpenAddAttendantWindow);
            AddParkingCompanyCommand = new RelayCommand(OpenAddParkingCompanyWindow);
            AddParkingLotCommand = new RelayCommand(OpenAddParkingLotWindow);
            AddReceiptCommand = new RelayCommand(OpedAddReceiptWindow);

            #endregion

            #region Инициализация команд изменения
            
            EditParkingCompanyCommand = new RelayCommand(EditParkingCompanyFunction);
            EditParkingLotCommand = new RelayCommand(EditParkingLotFunction);
            EditAttendantCommand = new RelayCommand(EditAttendantFunction);
            EditOwnerCommand = new RelayCommand(EditOwnerFunction);

            #endregion
            
            #region Инициализация команд сохранения

            SaveToExcelCommand = new RelayCommand(SaveToExcel);
            SaveToTextFileCommand = new RelayCommand(SaveToTextFile);
            /*Вывод квитанции для печати*/
            PrintReceiptCommand = new RelayCommand(PrintReceipt);
            /*Вывод статистики квитанций по месяцам*/
            GetReceiptStatsCommand = new RelayCommand(GetReceiptStatistics);
            /*Закрытие приложение*/
            CloseApplicationCommand = new RelayCommand(CloseApplication);

            #endregion
            
            //Предзагрузка данных из Excel
            _ = LoadExcelDatas();
        }

        #region Предзагрузка данных из Excel

        private async Task LoadExcelDatas()
        {
            await LoadAttendantsFromExcel();
            await LoadOwnersFromExcel();
            await LoadParkingLotsFromExcel();
            await LoadParkingFromExcel();
            await LoadVehiclesFromExcel();
            await LoadReceiptsFromExcel();
        }

        private async Task LoadAttendantsFromExcel()
        {
            var filePath = _filePath;
            if (File.Exists(filePath))
            {
                var attendants = await _excelDataLoaderService.LoadAttendantsFromExcel(filePath);
                foreach (var attendant in attendants) Attendants.Add(attendant);
            }
            else
            {
                MessageBox.Show("Файл Excel не найден.", "Ошибка");
            }
        }

        private async Task LoadOwnersFromExcel()
        {
            var filePath = _filePath;
            if (File.Exists(filePath))
            {
                var owners = await _excelDataLoaderService.LoadOwnersFromExcel(filePath);
                foreach (var owner in owners) Owners.Add(owner);
            }
            else
            {
                MessageBox.Show("Файл Excel не найден.", "Ошибка");
            }
        }

        private async Task LoadParkingLotsFromExcel()
        {
            var filePath = _filePath;
            if (File.Exists(filePath))
            {
                var parkingLots = await _excelDataLoaderService.LoadParkingLotsFromExcel(filePath);
                foreach (var parkingLot in parkingLots) ParkingLots.Add(parkingLot);
            }
            else
            {
                MessageBox.Show("Файл Excel не найден.", "Ошибка");
            }
        }

        private async Task LoadParkingFromExcel()
        {
            var filePath = _filePath;
            if (File.Exists(filePath))
            {
                var parkings = await _excelDataLoaderService.LoadParkingFromExcel(filePath);
                foreach (var parking in parkings) ParkingsCompany.Add(parking);
            }
            else
            {
                MessageBox.Show("Файл Excel не найден.", "Ошибка");
            }
        }

        private async Task LoadVehiclesFromExcel()
        {
            var filePath = _filePath;
            if (File.Exists(filePath))
            {
                var vehicles = await _excelDataLoaderService.LoadVehicleFromExcel(filePath);
                foreach (var vehicle in vehicles) Vehicles.Add(vehicle);
            }
            else
            {
                MessageBox.Show("Файл Excel не найден.", "Ошибка");
            }
        }
        
        private async Task LoadReceiptsFromExcel()
        {
            var filePath = _filePath;
            if (File.Exists(filePath))
            {
                var receipts = await _excelDataLoaderService.LoadReceiptFromExcel(filePath, Owners, ParkingsCompany,
                    ParkingLots, Attendants);
                foreach (var receipt in receipts) Receipts.Add(receipt);
            }
            else
            {
                MessageBox.Show("Файл Excel не найден.", "Ошибка");
            }
        }

        #endregion

        #region Функции для команд добавления

        /// <summary>
        /// Окно для добавления клиентов
        /// </summary>
        private void OpenAddOwnerWindow(object parameter)
        {
            var viewModel = new AddOwnerViewModel(_ownerService);
            var window = new AddOwnerWindow(viewModel);
            window.ShowDialog();
        }

        /// <summary>
        /// окно для добавления кладовщика
        /// </summary>
        private void OpenAddAttendantWindow(object parameter)
        {
            var viewModel = new AddAttendantViewModel(_attendantService);
            var window = new AddAttendantWindow(viewModel);
            window.ShowDialog();
        }

        /// <summary>
        /// окно для добавления стоянки
        /// </summary>
        private void OpenAddParkingCompanyWindow(object parameter)
        {
            var viewModel = new AddParkingViewModel(_parkingService);
            var window = new AddParkingWindow(viewModel);
            window.ShowDialog();
        }

        /// <summary>
        /// окно для добавления парковочного места
        /// </summary>
        private void OpenAddParkingLotWindow(object parameter)
        {
            var viewModel = new AddParkingLotViewModel(_parkingLotService, ParkingsCompany);
            var window = new AddParkingLotWindow(viewModel);
            window.ShowDialog();
        }

        /// <summary>
        /// окно для добавления квитанции
        /// </summary>
        private void OpedAddReceiptWindow(object parameter)
        {
            var viewModel = new AddReceiptViewModel(_receiptService, Receipts, ParkingsCompany, ParkingLots, Owners,
                Attendants, Vehicles);
            var window = new AddReceiptWindow(viewModel);
            window.ShowDialog();
        }

        #endregion

        #region Функции для команд изменения

        /// <summary>
        /// окно для изменения инф. о стоянке
        /// </summary>
        private void EditParkingCompanyFunction(object parameter)
        {
            if (!(parameter is Parkings parking)) return;
            var viewModel = new EditParkingViewModel(parking, _parkingService, ParkingsCompany);
            var window = new EditParkingWindow(viewModel);
            window.ShowDialog();
        }
        
        /// <summary>
        /// окно для изменения инф. о парковочном месте
        /// </summary>
        private void EditParkingLotFunction(object parameter)
        {
            if (!(parameter is ParkingLots parking)) return;
            var viewModel = new EditParkingLotViewModel(parking, _parkingLotService, ParkingLots);
            var window = new EditParkingLotWindow(viewModel);
            window.ShowDialog();
        }

        /// <summary>
        /// окно для изменения инф. о кладощике
        /// </summary>
        private void EditAttendantFunction(object parameter)
        {
            if (!(parameter is Attendants attendant)) return;
            var viewModel = new EditAttendantViewModel(attendant, _attendantService, Attendants);
            var window = new EditAttendantWindow(viewModel);
            window.ShowDialog();
        }
        
        /// <summary>
        /// окно для изменения инф. о клиенте
        /// </summary>
        private void EditOwnerFunction(object parameter)
        {
            if (!(parameter is Owners owner)) return;
            var viewModel = new EditOwnerViewModel(owner, _ownerService, Owners);
            var window = new EditOwnerWindow(viewModel);
            window.ShowDialog();
        }

        #endregion

        /// <summary>
        /// Сохранить данные в Excel
        /// </summary>
        private void SaveToExcel(object parameter)
        {
            // Реализуйте логику сохранения в Excel
            // Например, вызов метода для экспорта данных в файл Excel
            Console.WriteLine("Data saved to Excel");
        }


        /// <summary>
        /// Сохранить данные в текстовый файл
        /// </summary>
        private void SaveToTextFile(object parameter)
        {
            // Реализуйте логику сохранения в текстовый файл
            // Например, сохранение данных в текстовый файл
            Console.WriteLine("Data saved to Text File");
        }

        /// <summary>
        /// Печать квитанции
        /// </summary>
        private void PrintReceipt(object parameter)
        {
            if (!(parameter is Receipts receipt)) return;

            var tags = new Dictionary<string, string>
            {
                { "<PARKINGNAME>", receipt.Parking.Name },
                { "<INNPARKING>", $"ИНН: {receipt.Parking.Inn}" },
                { "<ADDRESSPARKING>", receipt.Parking.Address },
                { "<SERIES>", receipt.Series },
                { "<NUMBER>", receipt.Number },
                { "<PARKINGLOT>", receipt.ParkingLot.Name },

                { "<CARBRAND>", receipt.Owner.Vehicles.FirstOrDefault(t => t.Id == receipt.SelectedCarId)?.Brand },
                { "<CARMODEL>", receipt.Owner.Vehicles.FirstOrDefault(t => t.Id == receipt.SelectedCarId)?.Model },
                {
                    "<LICENSEPLATE>",
                    receipt.Owner.Vehicles.FirstOrDefault(t => t.Id == receipt.SelectedCarId)?.LicensePlate
                },

                { "<FIOOWNER>", receipt.Owner.FullNameInLine },
                { "<ADDRESSOWNER>", receipt.Owner.Address },
                { "<PHONEOWNER>", receipt.Owner.Phone },

                { "<STARTDATE>", receipt.GetStartDate().ToShortDateString() },
                { "<SHOUR>", receipt.GetStartDate().ToString("HH") },
                { "<SMINE>", receipt.GetStartDate().ToString("mm") },
                { "<STARTPARK>", receipt.GetStartDate().ToString("dd.MM.yyyy") },
                { "<ENDPARK>", receipt.EndDate.ToString("dd.MM.yyyy") },

                { "<FIOATTENDANT>", receipt.Attendants.FullNameInLine },

                { "<DAYS>", receipt.Days.ToString() },
                { "<AMOUNT>", receipt.Amount.ToString() }
            };

            var wordService = new WordService();
            var outputWordPath = wordService.GenerateReceipt(tags);
            var pdfFilePath = wordService.ConvertWordToPdf(outputWordPath);

            var saveFileDialog = new SaveFileDialog
            {
                Title = "Сохранить квитанцию",
                Filter = "PDF файлы (*.pdf)|*.pdf",
                FileName =
                    $"Квитанция_{receipt.Series}_{receipt.Number}_{receipt.Parking.Name.Replace(" ", "_").Replace("\"", "_")}_{receipt.GetStartDate().ToString("dd.MM.yyyy")}.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
                try
                {
                    File.Copy(pdfFilePath, saveFileDialog.FileName, true);
                    MessageBox.Show("Квитанция успешно сохранена!", "Успех", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
        }

        /// <summary>
        /// Вывод статистики по суммам квитанций по месяцам в виде гистограммы.
        /// </summary>
        private void GetReceiptStatistics(object parameter)
        {
            var receiptsToStat = Receipts;
            var statistics = ReceiptStatisticsService.GetMonthlyStatistics(receiptsToStat);
            var viewModel = new StatisticsViewModel(statistics);

            var statisticsWindow = new StatisticsWindow(viewModel);
            statisticsWindow.ShowDialog();
        }

        /// <summary>
        /// Закрытие приложения
        /// </summary>
        private void CloseApplication(object parameter)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Реализация INotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private async void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            await _excelDataSaverService.SaveDataToExcelAsync(Owners, ParkingsCompany, ParkingLots, Attendants, Receipts);
        }

        #region Обработчик событий ADD

        private async void OnAttendantAdded(Attendants attendant)
        {
            Attendants.Add(attendant);
            await SaveDataToExcelAsync();
        }

        private async void OnParkingAdded(Parkings parking)
        {
            ParkingsCompany.Add(parking);
            await SaveDataToExcelAsync();
        }

        private async void OnParkingLotAdded(ParkingLots parkingLot)
        {
            ParkingLots.Add(parkingLot);
            await SaveDataToExcelAsync();
        }

        private async void OnOwnerAdded(Owners owner)
        {
            Owners.Add(owner);
            await SaveDataToExcelAsync();
        }
        
        private async void OnReceiptAdded(Receipts receipts)
        {
            Receipts.Add(receipts);
            ParkingLots.FirstOrDefault(pl => pl.Id == receipts.ParkingLot.Id).IsFree = false;
            OnPropertyChanged(nameof(ParkingLots));
            await SaveDataToExcelAsync();
        }

        private async Task SaveDataToExcelAsync()
        {
            await _excelDataSaverService.SaveDataToExcelAsync(Owners, ParkingsCompany, ParkingLots, Attendants, Receipts);
        }

        #endregion
    }
}