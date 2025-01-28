using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ParkingWork.Entities.Parking;
using ParkingWork.Exceptions;
using ParkingWork.Services;

namespace ParkingWork.ViewModels.Edits
{
    public class EditParkingLotViewModel : INotifyPropertyChanged
    {
        private string _name;
        private string _isFree;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string IsFree
        {
            get => _isFree;
            set
            {
                _isFree = value;
                OnPropertyChanged(nameof(IsFree));
            }
        }

        public ICommand ChangeCommand { get; }
        public ICommand RedirectBackCommand { get; }

        // ParkingLot collection
        public ObservableCollection<ParkingLots> ParkingLots { get; set; }
        // Changing parkingLot
        private readonly ParkingLots _parkingLotChange;
        // ParkingLot service
        private readonly ParkingLotService _parkingLotService;

        public EditParkingLotViewModel(ParkingLots parkingToChange, ParkingLotService parkingService,
            ObservableCollection<ParkingLots> parkings)
        {
            _parkingLotChange = parkingToChange;
            _parkingLotService = parkingService;
            ParkingLots = parkings;

            Name = _parkingLotChange.Name;
            IsFree = _parkingLotChange.IsFree == true ? "Свободно" : "Занято";
            
            ChangeCommand = new RelayCommand(Change);
            RedirectBackCommand = new RelayCommand(RedirectBack);
        }

        private void Change(object parameter)
        {
            try
            {
                if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(IsFree))
                    throw new ArgumentNullException("Поля не могут быть пустыми");

                ParkingLots.FirstOrDefault(t => t.Id == _parkingLotChange.Id)
                    ?.ChangeParkingLot(Name, IsFree == "Свободно" ? true : false);

                _parkingLotService.EditParkingLot(ParkingLots, _parkingLotChange.Id);

                ParkingException.ShowSuccessMessage("Парковочное место успешно изменено!");
                
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