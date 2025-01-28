using System;
using System.Windows;

namespace ParkingWork.Exceptions
{
    public class ParkingException : Exception
    {
        public ParkingException(string message) : base(message) { }
        
        public static void ShowSuccessMessage(string message)
        {
            MessageBox.Show(message, "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}