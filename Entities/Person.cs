using System;
using System.Collections.Generic;

namespace ParkingWork.Entities
{
    public class Person
    {
        public Person(string name, string surname, string patronymic)
        {
            Name = name;
            Surname = surname;
            Patronymic = patronymic;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        
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
        
        public string FullNameInLine
        {
            get
            {
                var fullName = new List<string>
                {
                    $"{Surname} {Name} {Patronymic}",
                };

                return string.Join(Environment.NewLine, fullName);
            }
        }
    }
}