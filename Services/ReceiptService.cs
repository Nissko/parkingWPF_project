using System;
using ParkingWork.Entities.Attendants;
using ParkingWork.Entities.Owner;
using ParkingWork.Entities.Parking;
using ParkingWork.Entities.Parking.Receipt;

namespace ParkingWork.Services
{
    public class ReceiptService
    {
        public event Action<Receipts> ReceiptAdded;

        public void AddReceipt(string series, string number, Owners owner, Parkings parking, ParkingLots parkingLot,
            Attendants attendant, int days, decimal price)
        {
            if (string.IsNullOrEmpty(series) || string.IsNullOrEmpty(number) || string.IsNullOrEmpty(days.ToString()) ||
                string.IsNullOrEmpty(price.ToString()) || owner == null || parking == null || parkingLot == null ||
                attendant == null)
            {
                throw new ArgumentException("Все поля должны быть заполнены.");
            }

            var newReceipt = new Receipts(series: series, number: number, owner: owner, parking: parking,
                parkingLot: parkingLot, attendants: attendant, days: days, price: price);

            ReceiptAdded?.Invoke(newReceipt);
        }
    }
}