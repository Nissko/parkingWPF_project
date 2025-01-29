using System;

namespace ParkingWork.Entities.Attendant
{
    /// <summary>
    /// Кладовщик
    /// </summary>
    public class Attendants : Person
    {
        public Attendants(Guid id, string name, string surname, string patronymic)
            : base(name, surname, patronymic)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Patronymic = patronymic;
        }

        public Guid Id { get; private set; }

        public void ChangeAttendant(string name, string surname, string patronymic)
        {
            Name = name;
            Surname = surname;
            Patronymic = patronymic;
        }
    }
}