using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ParkingWork.Entities.Attendants;
using ParkingWork.Entities.Owner;
using ParkingWork.Entities.Parking;
using ParkingWork.Services;
using ParkingWork.ViewModels.Adds;
using ParkingWork.ViewModels.Edits;
using ParkingWork.Windows.Adds;
using ParkingWork.Windows.Edits;

namespace ParkingWork.ViewModels
{
    /// <summary>
    /// Главная модель парковки
    /// </summary>
    public class ParkingViewModel : INotifyPropertyChanged
    {
        // Collections
        public ObservableCollection<Parkings> ParkingsCompany { get; set; }
        public ObservableCollection<ParkingLots> ParkingLots { get; set; }
        public ObservableCollection<Owners> Owners { get; set; }
        public ObservableCollection<Attendants> Attendants { get; set; }

        // Add Commands
        public ICommand AddOwnerCommand { get; }
        public ICommand AddAttendantCommand { get; }
        public ICommand AddParkingCompanyCommand { get; }
        public ICommand AddParkingLotCommand { get; }

        // Edit Commands
        public ICommand EditParkingCompanyCommand { get; }
        public ICommand EditParkingLotCommand { get; }
        public ICommand EditAttendantCommand { get; }

        // Save Commands
        public ICommand SaveToExcelCommand { get; }
        public ICommand SaveToTextFileCommand { get; }
        public ICommand PrintCommand { get; }
        public ICommand CloseApplicationCommand { get; }

        // Services
        private readonly AttendantService _attendantService;
        private readonly ParkingService _parkingService;
        private readonly ParkingLotService _parkingLotService;
        private readonly OwnerService _ownerService;
        private readonly ExcelDataLoaderService _excelDataLoaderService;

        public ParkingViewModel()
        {
            // Init collections
            ParkingsCompany = new ObservableCollection<Parkings>();
            ParkingLots = new ObservableCollection<ParkingLots>();
            Owners = new ObservableCollection<Owners>();
            Attendants = new ObservableCollection<Attendants>();

            // Init services
            _attendantService = new AttendantService();
            _parkingService = new ParkingService();
            _parkingLotService = new ParkingLotService();
            _ownerService = new OwnerService();
            _excelDataLoaderService = new ExcelDataLoaderService();

            // ADD Events
            _attendantService.AttendantAdded += attendant => Attendants.Add(attendant);
            _parkingService.ParkingAdded += parking => ParkingsCompany.Add(parking);
            _parkingLotService.ParkingLotAdded += parkingLot => ParkingLots.Add(parkingLot);
            _ownerService.OwnerAdded += owner => Owners.Add(owner);

            // EDIT Events
            // TODO: предусмотреть изменения в других коллекциях (проверить)
            _parkingService.ParkingChanged += parking =>
            {
                if (parking == null) return;
                var updatedList = new ObservableCollection<Parkings>(ParkingsCompany);

                var existingParking = updatedList.FirstOrDefault(p => p.Id == parking.Id);
                if (existingParking != null)
                    existingParking.ChangeParking(parking.Name, parking.Address);
                else
                    updatedList.Add(parking);

                ParkingsCompany = updatedList;
                OnPropertyChanged(nameof(ParkingsCompany));
                
                //Update ParkingLotsCollection
                var updatedParkingLotsList = new ObservableCollection<ParkingLots>(ParkingLots.Select(parkingLot =>
                {
                    if (parkingLot.ParkingId == parking.Id)
                    {
                        parkingLot.ChangeParkingName(parking.Name);
                    }
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
                
                //Update ParkingLotsCollection
                var updatedParkingLotsList = new ObservableCollection<ParkingLots>(ParkingLots.Select(parkingLot =>
                {
                    if (parkingLot.ParkingId == parking.Id)
                    {
                        parkingLot.ChangeParkingName(parking.Name);
                    }
                    return parkingLot;
                }));

                ParkingLots = updatedParkingLotsList;
                OnPropertyChanged(nameof(ParkingLots));
            };

            // Init Add Commands 
            AddOwnerCommand = new RelayCommand(OpenAddOwnerWindow);
            AddAttendantCommand = new RelayCommand(OpenAddAttendantWindow);
            AddParkingCompanyCommand = new RelayCommand(OpenAddParkingCompanyWindow);
            AddParkingLotCommand = new RelayCommand(OpenAddParkingLotWindow);

            // Init Edit Commands
            EditParkingCompanyCommand = new RelayCommand(EditParkingCompanyFunction);
            EditParkingLotCommand = new RelayCommand(EditParkingLotFunction);
            EditAttendantCommand = new RelayCommand(EditAttendantFunction);

            // Init Save Commands
            SaveToExcelCommand = new RelayCommand(SaveToExcel);
            SaveToTextFileCommand = new RelayCommand(SaveToTextFile);
            PrintCommand = new RelayCommand(Print);
            CloseApplicationCommand = new RelayCommand(CloseApplication);

            // Load Excel Data
            _ = LoadExcelDatas();
        }

        #region Preload Data from Excel

        private async Task LoadExcelDatas()
        {
            await LoadAttendantsFromExcel();
            await LoadOwnersFromExcel();
            await LoadParkingLotsFromExcel();
            await LoadParkingFromExcel();
        }

        private async Task LoadAttendantsFromExcel()
        {
            var filePath = "C:\\UniversityFiles\\Курсовая работа ЯП\\ParkingWork\\DbFolder\\ParkingDB.xlsx";
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
            var filePath = "C:\\UniversityFiles\\Курсовая работа ЯП\\ParkingWork\\DbFolder\\ParkingDB.xlsx";
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
            var filePath = "C:\\UniversityFiles\\Курсовая работа ЯП\\ParkingWork\\DbFolder\\ParkingDB.xlsx";
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
            var filePath = "C:\\UniversityFiles\\Курсовая работа ЯП\\ParkingWork\\DbFolder\\ParkingDB.xlsx";
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

        #endregion

        #region Add Commands

        /// <summary>
        ///     Окно для добавления клиентов
        /// </summary>
        private void OpenAddOwnerWindow(object parameter)
        {
            var viewModel = new AddOwnerViewModel(_ownerService);
            var window = new AddOwnerWindow(viewModel);
            window.ShowDialog();
        }

        /// <summary>
        ///     окно для добавления кладовщика
        /// </summary>
        private void OpenAddAttendantWindow(object parameter)
        {
            var viewModel = new AddAttendantViewModel(_attendantService);
            var window = new AddAttendantWindow(viewModel);
            window.ShowDialog();
        }

        /// <summary>
        ///     окно для добавления стоянки
        /// </summary>
        private void OpenAddParkingCompanyWindow(object parameter)
        {
            var viewModel = new AddParkingViewModel(_parkingService);
            var window = new AddParkingWindow(viewModel);
            window.ShowDialog();
        }

        /// <summary>
        ///     окно для добавления парковочного места
        /// </summary>
        private void OpenAddParkingLotWindow(object parameter)
        {
            var viewModel = new AddParkingLotViewModel(_parkingLotService, ParkingsCompany);
            var window = new AddParkingLotWindow(viewModel);
            window.ShowDialog();
        }

        #endregion

        #region Edit Commands

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
        /// окно для изменения инф. о кладвщике
        /// </summary>
        private void EditAttendantFunction(object parameter)
        {
            if (!(parameter is Attendants attendant)) return;
            var viewModel = new EditAttendantViewModel(attendant, _attendantService, Attendants);
            var window = new EditAttendantWindow(viewModel);
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
        /// Напечатать данные
        /// </summary>
        private void Print(object parameter)
        {
            // Реализуйте логику печати данных
            // Например, вызов метода для печати данных
            Console.WriteLine("Print executed");
        }

        private void CloseApplication(object parameter)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Реализация INotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}