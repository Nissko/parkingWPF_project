using System;
using ParkingWork.Entities.Owner;

namespace ParkingWork.Entities.Parking.Receipt
{
    /// <summary>
    /// Квитанция о стоянке
    /// </summary>
    public class Receipts
    {
        public Receipts(string series, string number, Owners owner, Parkings parking,
            ParkingLots parkingLot, Attendants.Attendants attendants, int days, decimal price, Guid selectedCarId, DateTime? startDate = null)
        {
            Id = Guid.NewGuid();
            Series = series;
            Number = number;
            Owner = owner;
            Parking = parking;
            ParkingLot = parkingLot;
            Attendants = attendants;
            Days = days;
            _price = price;
            _startDate = startDate ?? DateTime.Now;
            EndDate = _startDate.AddDays(days);
            _selectedCarId = selectedCarId;
            CalculateAmount(_price);
        }
        
        public Receipts(Guid id, string series, string number, Owners owner, Parkings parking,
            ParkingLots parkingLot, Attendants.Attendants attendants, int days, decimal price, Guid selectedCarId, DateTime? startDate = null)
        {
            Id = id;
            Series = series;
            Number = number;
            Owner = owner;
            Parking = parking;
            ParkingLot = parkingLot;
            Attendants = attendants;
            Days = days;
            _price = price;
            _startDate = startDate ?? DateTime.Now;
            EndDate = _startDate.AddDays(days);
            _selectedCarId = selectedCarId;
            CalculateAmount(_price);
        }

        public Guid Id { get; set; }
        
        /// <summary>
        /// Серия
        /// </summary>
        public string Series { get; set; }
        
        /// <summary>
        /// Номер
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Дата формирования квитанции
        /// </summary>
        private DateTime _startDate;
        
        /// <summary>
        /// Дата окончания парковки
        /// </summary>
        public DateTime EndDate { get; set; }
        
        /// <summary>
        /// Клиент
        /// </summary>
        public Owners Owner { get; set; }
        
        /// <summary>
        /// Парковка
        /// </summary>
        public Parkings Parking { get; set; }
        
        /// <summary>
        /// Парковочное место
        /// </summary>
        public ParkingLots ParkingLot { get; set; }
        
        /// <summary>
        /// Кладовщик
        /// </summary>
        public Attendants.Attendants Attendants { get; set; }
        
        /// <summary>
        /// Кол-во дней
        /// </summary>
        public int Days { get; set; }

        /// <summary>
        /// Цена
        /// </summary>
        private decimal _price;
        public decimal Price => _price;

        /// <summary>
        /// Стоимость
        /// </summary>
        private decimal _amount;
        public decimal Amount => _amount;

        /// <summary>
        /// Id выбранного авто
        /// </summary>
        private Guid _selectedCarId;
        public Guid SelectedCarId => _selectedCarId;

        /// <summary>
        /// Расчет стоимости
        /// </summary>
        public void CalculateAmount(decimal dailyRate)
        {
            _amount = Days * dailyRate;
        }

        public DateTime GetStartDate()
        {
            return _startDate;
        }
    }
}