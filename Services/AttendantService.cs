using System;
using System.Collections.ObjectModel;
using System.Linq;
using ParkingWork.Entities.Attendants;

namespace ParkingWork.Services
{
    public class AttendantService
    {
        public event Action<Attendants> AttendantAdded;
        public event Action<Attendants> AttendantChanged;

        public void AddAttendant(string name, string surname, string patronymic)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(surname) ||
                string.IsNullOrWhiteSpace(patronymic)) throw new ArgumentException("Все поля должны быть заполнены.");

            var newAttendant = new Attendants(Guid.NewGuid(), name, surname, patronymic);

            AttendantAdded?.Invoke(newAttendant);
        }

        public void EditAttendant(ObservableCollection<Attendants> attendants, Guid id)
        {
            if (!attendants.Any()) throw new ArgumentNullException("Не удалось получить данные по кладовщикам.");
            
            var attendant = attendants.FirstOrDefault(x => x.Id == id);
            
            AttendantChanged?.Invoke(attendant);
        }
    }
}