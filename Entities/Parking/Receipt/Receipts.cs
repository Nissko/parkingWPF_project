using System;
using ParkingWork.Entities.Owner;

namespace ParkingWork.Entities.Parking.Receipt
{
    /// <summary>
    /// Квитанция о стоянке
    /// </summary>
    public class Receipts
    {
        public Guid Id { get; set; }
        public string Series { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public Owners Owner { get; set; }
        public Parkings Parking { get; set; }
        public ParkingLots ParkingLot { get; set; }
        public Attendants.Attendants Attendants { get; set; }
        public int Days { get; set; }
        public decimal Amount { get; set; }

        public void CalculateAmount(decimal dailyRate)
        {
            Amount = Days * dailyRate;
        }
    }
}