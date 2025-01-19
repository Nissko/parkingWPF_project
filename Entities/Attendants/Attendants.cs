using System;
using System.Collections.Generic;

namespace ParkingWork.Entities.Attendants
{
    /// <summary>
    /// Кладовщик
    /// </summary>
    /// TODO: Сделать разделение, если мы хотим выбрать кладовщиков из определенной парковки
    public class Attendants
    {
        public Attendants(Guid id, string name, string surname, string patronymic)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Patronymic = patronymic;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public string Patronymic { get; private set; }
        
        public string FullName
        {
            get
            {
                var fullName = new List<string>
                {
                    $"Фамилия: {Surname}",
                    $"Имя: {Name}",
                    $"Отчество: {Patronymic}",
                };

                return string.Join(Environment.NewLine, fullName);
            }
        }

        public void ChangeAttendant(string name, string surname, string patronymic)
        {
            Name = name;
            Surname = surname;
            Patronymic = patronymic;
        }
    }
}