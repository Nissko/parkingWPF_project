using System;
using System.Collections.ObjectModel;
using System.Linq;
using ParkingWork.Entities.Parking;

namespace ParkingWork.Services
{
    public class ParkingService
    {
        public event Action<Parkings> ParkingAdded;
        
        public event Action<Parkings> ParkingChanged;

        public void AddParking(string parkingName, string parkingAddress, int inn)
        {
            if (string.IsNullOrEmpty(parkingName) || string.IsNullOrEmpty(parkingAddress) || string.IsNullOrEmpty(inn.ToString()))
            {
                throw new ArgumentException("Все поля должны быть заполнены.");
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