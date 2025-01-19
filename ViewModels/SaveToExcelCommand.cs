using System;

namespace ParkingWork.ViewModels
{
    public class SaveToExcelCommand
    {
        public static void Execute(object parameter)
        {
            Console.WriteLine("Data saved to Excel");
        }
    }
}