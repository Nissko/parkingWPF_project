using System;
using System.Collections.Generic;
using System.Linq;
using ParkingWork.Entities.Vehicle;

namespace ParkingWork.Entities.Owner
{
    /// <summary>
    /// Клиенты / Владельцы
    /// </summary>
    public class Owners : Person
    {
        public Owners(Guid id, string name, string surname, string patronymic, string address, string phone,
        IEnumerable<Vehicles> vehicles) : base(name, surname, patronymic)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Patronymic = patronymic;
            Address = address;
            Phone = phone;
            Vehicles = vehicles;
        }

        public Guid Id { get; private set; }
        
        public string Address { get; private  set; }
        public string Phone { get; private  set; }
        public IEnumerable<Vehicles> Vehicles { get; set; }

        public string ContactInformation
        {
            get
            {
                var contactInformation = new List<string>
                {
                    $"Фамилия: {Surname}",
                    $"Имя: {Name}",
                    $"Отчество: {Patronymic}",
                    $"Адрес: {Address}",
                    $"Телефон: {Phone}",
                    $"Автомобили: {string.Join(", ", Vehicles.Select(v => $"{v.Brand} {v.Model} ({v.LicensePlate})"))}"
                };

                return string.Join(Environment.NewLine, contactInformation);
            }
        }

        /*Функция для изменения Owner*/
        public void ChangeOwner(string newName, string newSurname, string newPatronymic, string newAddress,
            string newPhone, IEnumerable<Vehicles> newVehicles)
        {
            Name = newName ?? Name;
            Surname = newSurname ?? Surname;
            Patronymic = newPatronymic ?? Patronymic;
            Address = newAddress ?? Address;
            Phone = newPhone ?? Phone;
            Vehicles = newVehicles ?? Vehicles;
        }
    }
}
