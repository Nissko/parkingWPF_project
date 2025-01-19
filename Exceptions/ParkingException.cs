using System;

namespace ParkingWork.Exceptions
{
    public class ParkingException : Exception
    {
        public ParkingException(string message) : base(message) { }
    }
}