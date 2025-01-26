using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ParkingWork.Entities.Parking.Receipt;

namespace ParkingWork.Services
{
    public class ReceiptStatisticsService
    {
        /// <summary>
        /// Получение статистики квитанций по месяцам
        /// </summary>
        public static List<ReceiptStatistics> GetMonthlyStatistics(ObservableCollection<Receipts> receipts)
        {
            return receipts
                .GroupBy(r => new { Year = r.GetStartDate().Year, Month = r.GetStartDate().Month })
                .Select(g => new ReceiptStatistics
                {
                    Month = $"{g.Key.Month:D2}.{g.Key.Year}",
                    TotalAmount = g.Sum(r => r.Amount)
                })
                .OrderBy(stat => new DateTime(
                    year: int.Parse(stat.Month.Split('.')[1]),
                    month: int.Parse(stat.Month.Split('.')[0]),
                    day: 1))
                .ToList();
        }
    }
}