using System;
using System.Collections.Generic;
using System.Linq;
using LiveCharts;
using ParkingWork.Entities.Parking.Receipt;

namespace ParkingWork.ViewModels.Stats
{
    /// <summary>
    /// Формирование данных гистограммы
    /// </summary>
    public class StatisticsViewModel
    {
        public ChartValues<decimal> Values { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        public StatisticsViewModel(List<ReceiptStatistics> statistics)
        {
            Values = new ChartValues<decimal>(statistics.Select(stat => stat.TotalAmount));
            Labels = statistics.Select(stat => stat.Month).ToArray();

            //Y-ось как денежные значения
            Formatter = value => value.ToString("C");
        }
    }
}