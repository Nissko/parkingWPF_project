using System;
using System.Collections.Generic;
using System.Linq;
using ParkingWork.Entities.Owner;
using ParkingWork.Entities.Vehicle;

namespace ParkingWork.Services
{
    public class OwnerService
    {
        public event Action<Owners> OwnerAdded;

        public void AddOwner(Guid clientId, string name, string surname, string patronymic, string address, string phone, IEnumerable<Vehicles> cars)
        {
            var vehiclesEnumerable = cars.ToList();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname) || string.IsNullOrEmpty(patronymic) ||
                string.IsNullOrEmpty(address) || string.IsNullOrEmpty(phone) || vehiclesEnumerable.Count == 0) 
            {
                throw new ArgumentException("Все поля должны быть заполнены.");
            }

            var newOwner = new Owners(clientId, name, surname, patronymic, address, phone, vehiclesEnumerable);
            
            OwnerAdded?.Invoke(newOwner);
        }
    }
}