using System;
using System.Collections.Generic;

namespace ParkingWork.Entities.Parking
{
    /// <summary>
    /// Парковочные места
    /// </summary>
    public class ParkingLots
    {
        /// <summary>
        /// Для вывода во вкладке "Парковочные места"
        /// </summary>
        public ParkingLots(Guid id, Guid parkingId, string name, string parkingName, bool isFree)
        {
            Id = id;
            ParkingId = parkingId;
            Name = name;
            _parkingName = parkingName;
            IsFree = isFree;
        }

        /*Public*/
        public Guid Id { get; set; }
        public Guid ParkingId { get; set; }
        public string Name { get; set; }
        public bool IsFree { get; set; }
        
        /*Private*/
        private string _parkingName;
        
        public string NormalizedName
        {
            get
            {
                var normalizedName = new List<string>
                {
                    $"Наименование места: {Name}",
                    $"Доступность: {(IsFree == true ? "Свободно" : "Занято")}",
                    $"Автостоянка: {_parkingName}"
                };

                return string.Join(Environment.NewLine, normalizedName);
            }
        }

        /// <summary>
        /// изменение имени парковки 
        /// </summary>
        public void ChangeParkingName(string name)
        {
            _parkingName = name;
        }

        /// <summary>
        /// изменение инф. парковочного места
        /// </summary>
        public void ChangeParkingLot(string name, bool isFree)
        {
            Name = name;
            IsFree = isFree;
        }
    }
}