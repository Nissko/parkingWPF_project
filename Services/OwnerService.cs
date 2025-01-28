using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ParkingWork.Entities.Owner;
using ParkingWork.Entities.Vehicle;
using ParkingWork.Exceptions;

namespace ParkingWork.Services
{
    public class OwnerService
    {
        public event Action<Owners> OwnerAdded;
        public event Action<Owners> OwnerChanged;

        public void AddOwner(Guid clientId, string name, string surname, string patronymic, string address, string phone, IEnumerable<Vehicles> cars)
        {
            var vehiclesEnumerable = cars.ToList();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname) || string.IsNullOrEmpty(patronymic) ||
                string.IsNullOrEmpty(address) || string.IsNullOrEmpty(phone) || vehiclesEnumerable.Count == 0) 
            {
                ParkingException.ShowErrorMessage("Все поля должны быть заполнены.");
                return;
            }

            var newOwner = new Owners(clientId, name, surname, patronymic, address, phone, vehiclesEnumerable);
            
            OwnerAdded?.Invoke(newOwner);
        }

        public void ChangeOwner(ObservableCollection<Owners> owners, Guid id)
        {
            var ownersList = owners.FirstOrDefault(x => x.Id == id);
            OwnerChanged?.Invoke(ownersList);
        }
    }
}