using System;
using System.Collections.Generic;
using ParkingWork.Entities.Parking.Receipt;

namespace ParkingWork.Entities.Parking
{
    /// <summary>
    /// Парковка
    /// </summary>
    public class Parkings
    {
        public Parkings(Guid id, string name, string address)
        {
            Id = id;
            Name = name;
            Address = address;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Address { get; private set; }
        
        /// <summary>
        /// Парковочные места стоянки
        /// </summary>
        public List<ParkingLots> ParkingLots { get; private set; } = new List<ParkingLots>();
        
        /// <summary>
        /// Квитанции на стоянку
        /// </summary>
        public List<Receipts> Receipts { get; private set; } = new List<Receipts>();
        
        public string ContactInformation
        {
            get
            {
                var contactInformation = new List<string>
                {
                    $"Название стоянки: {Name}",
                    $"Адрес местонахождения: {Address}",
                };

                return string.Join(Environment.NewLine, contactInformation);
            }
        }

        public void ChangeParking(string newName, string newAddress)
        {
            Name = newName ?? throw new ArgumentNullException(nameof(newName));
            Address = newAddress ?? throw new ArgumentNullException(nameof(newAddress));
        }
    }
}