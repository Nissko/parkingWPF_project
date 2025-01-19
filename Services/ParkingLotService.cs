using System;
using System.Collections.ObjectModel;
using System.Linq;
using ParkingWork.Entities.Parking;

namespace ParkingWork.Services
{
    public class ParkingLotService
    {
        public event Action<ParkingLots> ParkingLotAdded;
        public event Action<ParkingLots> ParkingLotChanged;

        public void AddParkingLot(string name, string parkingId, string parkingName)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(parkingId))
            {
                throw new ArgumentException("Все поля должны быть заполнены.");
            }

            var newParkingLot = new ParkingLots(Guid.NewGuid(), Guid.Parse(parkingId), name, parkingName, true);
            
            ParkingLotAdded?.Invoke(newParkingLot);
        }

        public void EditParkingLot(ObservableCollection<ParkingLots> changedParkingLot, Guid id)
        {
            var parkingLots = changedParkingLot.FirstOrDefault(x => x.Id == id);
            ParkingLotChanged?.Invoke(parkingLots);
        }
    }
}