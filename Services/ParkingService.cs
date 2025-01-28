using System;
using System.Collections.ObjectModel;
using System.Linq;
using ParkingWork.Entities.Parking;
using ParkingWork.Exceptions;

namespace ParkingWork.Services
{
    public class ParkingService
    {
        public event Action<Parkings> ParkingAdded;

        public event Action<Parkings> ParkingChanged;

        public void AddParking(string parkingName, string parkingAddress, int inn)
        {
            if (string.IsNullOrEmpty(parkingName) || string.IsNullOrEmpty(parkingAddress) ||
                string.IsNullOrEmpty(inn.ToString()))
            {
                ParkingException.ShowErrorMessage("Все поля должны быть заполнены.");
                return;
            }

            var newParking = new Parkings(Guid.NewGuid(), parkingName, parkingAddress, inn);

            ParkingAdded?.Invoke(newParking);
        }

        public void EditParking(ObservableCollection<Parkings> changedParking, Guid id)
        {
            var parking = changedParking.FirstOrDefault(x => x.Id == id);
            ParkingChanged?.Invoke(parking);
        }
    }
}