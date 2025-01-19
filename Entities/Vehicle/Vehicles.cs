using System;
using ParkingWork.Entities.Vehicle.Enum;

namespace ParkingWork.Entities.Vehicle
{
    /// <summary>
    /// Автомобили
    /// </summary>
    public class Vehicles
    {
        public Vehicles(Guid id, Guid clientId, string licensePlate, string brand, string model, VehicleColorEnums color)
        {
            Id = id;
            ClientId = clientId;
            LicensePlate = licensePlate;
            Brand = brand;
            Model = model;
            Color = color;
        }

        public Guid Id { get; private set; }
        public Guid ClientId { get; private set; }
        public string LicensePlate { get; private set; }
        public string Brand {  get; private set; }
        public string Model { get; private set; }
        public VehicleColorEnums Color { get; private set; }
    }
}
