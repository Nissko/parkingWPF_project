using System;
using System.Collections.ObjectModel;
using System.Linq;
using ParkingWork.Entities.Attendants;
using ParkingWork.Exceptions;

namespace ParkingWork.Services
{
    public class AttendantService
    {
        public event Action<Attendants> AttendantAdded;
        public event Action<Attendants> AttendantChanged;

        public void AddAttendant(string name, string surname, string patronymic)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(surname) ||
                string.IsNullOrWhiteSpace(patronymic))
            {
                ParkingException.ShowErrorMessage("Все поля должны быть заполнены.");
                return;
            }

            var newAttendant = new Attendants(Guid.NewGuid(), name, surname, patronymic);

            AttendantAdded?.Invoke(newAttendant);
        }

        public void EditAttendant(ObservableCollection<Attendants> attendants, Guid id)
        {
            if (!attendants.Any())
            {
                ParkingException.ShowErrorMessage("Не удалось получить данные по кладовщикам.");
                return;
            }
            
            var attendant = attendants.FirstOrDefault(x => x.Id == id);
            
            AttendantChanged?.Invoke(attendant);
        }
    }
}