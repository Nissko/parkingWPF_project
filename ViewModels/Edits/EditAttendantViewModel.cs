using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ParkingWork.Entities.Attendants;
using ParkingWork.Exceptions;
using ParkingWork.Services;

namespace ParkingWork.ViewModels.Edits
{
    public class EditAttendantViewModel : INotifyPropertyChanged
    {
        private string _name;
        private string _surname;
        private string _patronymic;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Surname
        {
            get => _surname;
            set
            {
                _surname = value;
                OnPropertyChanged(nameof(Surname));
            }
        }
        
        public string Patronymic
        {
            get => _patronymic;
            set
            {
                _patronymic = value;
                OnPropertyChanged(nameof(Patronymic));
            }
        }

        public ICommand ChangeCommand { get; }
        public ICommand RedirectBackCommand { get; }

        // Attendant collection
        public ObservableCollection<Attendants> Attendants { get; set; }
        // Changing attendant
        private readonly Attendants _attendantChange;
        // Attendant service
        private readonly AttendantService _attendantService;

        public EditAttendantViewModel(Attendants attendantChange, AttendantService attendantService,
            ObservableCollection<Attendants> attendants)
        {
            _attendantChange = attendantChange;
            _attendantService = attendantService;
            Attendants = attendants;
            
            Surname = _attendantChange.Surname;
            Name = _attendantChange.Name;
            Patronymic = _attendantChange.Patronymic;
            
            ChangeCommand = new RelayCommand(Change);
            RedirectBackCommand = new RelayCommand(RedirectBack);
        }

        private void Change(object parameter)
        {
            try
            {
                if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Surname) || string.IsNullOrEmpty(Patronymic))
                    throw new ArgumentNullException("Поля не могут быть пустыми");

                Attendants.FirstOrDefault(t => t.Id == _attendantChange.Id)?.ChangeAttendant(Name, Surname, Patronymic);

                _attendantService.EditAttendant(Attendants, _attendantChange.Id);

                ParkingException.ShowSuccessMessage("Кладовщик успешно изменен!");

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