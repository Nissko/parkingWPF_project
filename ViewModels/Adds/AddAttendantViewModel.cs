using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using ParkingWork.Exceptions;
using ParkingWork.Services;

namespace ParkingWork.ViewModels.Adds
{
    public class AddAttendantViewModel : INotifyPropertyChanged
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

        public ICommand SaveCommand { get; }
        public ICommand RedirectBackCommand { get; }

        private readonly AttendantService _attendantService;

        public AddAttendantViewModel(AttendantService attendantService)
        {
            _attendantService = attendantService;
            SaveCommand = new RelayCommand(Save);
            RedirectBackCommand = new RelayCommand(RedirectBack);
        }

        private void Save(object parameter)
        {
            try
            {
                if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Surname) || string.IsNullOrEmpty(Patronymic))
                {
                    ParkingException.ShowErrorMessage("Заполните все поля!");
                    return;
                }

                _attendantService.AddAttendant(Name, Surname, Patronymic);
                ParkingException.ShowSuccessMessage("Кладовщик успешно добавлен!");

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