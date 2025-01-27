using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using ParkingWork.Entities.Attendants;
using ParkingWork.Entities.Owner;
using ParkingWork.Entities.Parking;
using ParkingWork.Entities.Parking.Receipt;

namespace ParkingWork.Services
{
    public class ExcelDataSaverService
    {
        private readonly string _filePath;

        public ExcelDataSaverService(string filePath)
        {
            _filePath = filePath;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task SaveDataToExcelAsync(ObservableCollection<Owners> owners,
            ObservableCollection<Parkings> parkings,
            ObservableCollection<ParkingLots> parkingLots, ObservableCollection<Attendants> attendants,
            ObservableCollection<Receipts> receipts)
        {
            FileInfo newFile = new FileInfo(_filePath);
            using (var package = new ExcelPackage(newFile))
            {
                await CreateOrUpdateOwnersSheetAsync(package, owners);
                await CreateOrUpdateParkingsSheetAsync(package, parkings);
                await CreateOrUpdateParkingLotsSheetAsync(package, parkingLots);
                await CreateOrUpdateAttendantsSheetAsync(package, attendants);
                await CreateOrUpdateVehiclesSheetAsync(package, owners);
                await CreateOrUpdateReceiptSheetAsync(package, receipts);

                await package.SaveAsync();
            }
        }

        public async Task SaveDataToExcelFreeFormatAsync(ObservableCollection<Owners> owners,
            ObservableCollection<Parkings> parkings,
            ObservableCollection<ParkingLots> parkingLots, ObservableCollection<Attendants> attendants,
            ObservableCollection<Receipts> receipts)
        {
            var directory = Path.GetDirectoryName(_filePath);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(_filePath);
            var fileExtension = Path.GetExtension(_filePath);
            var newFilePath = Path.Combine(directory, $"{fileNameWithoutExtension}_FreeFormat{fileExtension}");

            FileInfo newFile = new FileInfo(newFilePath);

            using (var package = new ExcelPackage(newFile))
            {
                await CreateOrUpdateOwnersSheetAsync(package, owners);
                await CreateOrUpdateParkingsSheetAsync(package, parkings);
                await CreateOrUpdateParkingLotsSheetAsync(package, parkingLots);
                await CreateOrUpdateAttendantsSheetAsync(package, attendants);
                await CreateOrUpdateVehiclesSheetAsync(package, owners);
                await CreateOrUpdateReceiptSheetAsync(package, receipts);
                /*статистика*/
                await LastMonthsTotal(package, receipts, parkings);

                await package.SaveAsync();
            }
        }

        #region Таблицы

        private async Task CreateOrUpdateOwnersSheetAsync(ExcelPackage package, ObservableCollection<Owners> owners)
        {
            var sheet = package.Workbook.Worksheets["Owners"] ?? package.Workbook.Worksheets.Add("Owners");

            sheet.Cells[1, 1].Value = "Id";
            sheet.Cells[1, 2].Value = "Name";
            sheet.Cells[1, 3].Value = "Surname";
            sheet.Cells[1, 4].Value = "Patronymic";
            sheet.Cells[1, 5].Value = "Address";
            sheet.Cells[1, 6].Value = "Phone";

            for (int i = 0; i < owners.Count; i++)
            {
                var owner = owners[i];
                sheet.Cells[i + 2, 1].Value = owner.Id;
                sheet.Cells[i + 2, 2].Value = owner.Name;
                sheet.Cells[i + 2, 3].Value = owner.Surname;
                sheet.Cells[i + 2, 4].Value = owner.Patronymic;
                sheet.Cells[i + 2, 5].Value = owner.Address;
                sheet.Cells[i + 2, 6].Value = owner.Phone;
            }

            await Task.CompletedTask;
        }

        private async Task CreateOrUpdateParkingsSheetAsync(ExcelPackage package, ObservableCollection<Parkings> parkings)
        {
            var sheet = package.Workbook.Worksheets["Parking"] ?? package.Workbook.Worksheets.Add("Parking");

            sheet.Cells[1, 1].Value = "Id";
            sheet.Cells[1, 2].Value = "Name";
            sheet.Cells[1, 3].Value = "Address";
            sheet.Cells[1, 4].Value = "Inn";

            for (int i = 0; i < parkings.Count; i++)
            {
                var parking = parkings[i];
                sheet.Cells[i + 2, 1].Value = parking.Id;
                sheet.Cells[i + 2, 2].Value = parking.Name;
                sheet.Cells[i + 2, 3].Value = parking.Address;
                sheet.Cells[i + 2, 4].Value = parking.Inn;
            }

            await Task.CompletedTask;
        }

        private async Task CreateOrUpdateParkingLotsSheetAsync(ExcelPackage package, ObservableCollection<ParkingLots> parkingLots)
        {
            var sheet = package.Workbook.Worksheets["ParkingLots"] ?? package.Workbook.Worksheets.Add("ParkingLots");

            sheet.Cells[1, 1].Value = "Id";
            sheet.Cells[1, 2].Value = "ParkingId";
            sheet.Cells[1, 3].Value = "Name";
            sheet.Cells[1, 4].Value = "IsFree";

            for (int i = 0; i < parkingLots.Count; i++)
            {
                var parkingLot = parkingLots[i];
                sheet.Cells[i + 2, 1].Value = parkingLot.Id;
                sheet.Cells[i + 2, 2].Value = parkingLot.ParkingId;
                sheet.Cells[i + 2, 3].Value = parkingLot.Name;
                sheet.Cells[i + 2, 4].Value = parkingLot.IsFree == true ? "1" : "0";
            }

            await Task.CompletedTask;
        }

        private async Task CreateOrUpdateAttendantsSheetAsync(ExcelPackage package, ObservableCollection<Attendants> attendants)
        {
            var sheet = package.Workbook.Worksheets["Attendants"] ?? package.Workbook.Worksheets.Add("Attendants");

            sheet.Cells[1, 1].Value = "Id";
            sheet.Cells[1, 2].Value = "Name";
            sheet.Cells[1, 3].Value = "Surname";
            sheet.Cells[1, 4].Value = "Patronymic";

            for (int i = 0; i < attendants.Count; i++)
            {
                var attendant = attendants[i];
                sheet.Cells[i + 2, 1].Value = attendant.Id;
                sheet.Cells[i + 2, 2].Value = attendant.Name;
                sheet.Cells[i + 2, 3].Value = attendant.Surname;
                sheet.Cells[i + 2, 4].Value = attendant.Patronymic;
            }

            await Task.CompletedTask;
        }

        private async Task CreateOrUpdateVehiclesSheetAsync(ExcelPackage package, ObservableCollection<Owners> owners)
        {
            var sheet = package.Workbook.Worksheets["Vehicles"] ?? package.Workbook.Worksheets.Add("Vehicles");

            sheet.Cells[1, 1].Value = "Id";
            sheet.Cells[1, 2].Value = "ClientId";
            sheet.Cells[1, 3].Value = "LicensePlate";
            sheet.Cells[1, 4].Value = "Brand";
            sheet.Cells[1, 5].Value = "Model";
            sheet.Cells[1, 6].Value = "Color";

            int rowIndex = 2;

            foreach (var owner in owners)
            {
                foreach (var vehicle in owner.Vehicles)
                {
                    sheet.Cells[rowIndex, 1].Value = vehicle.Id;
                    sheet.Cells[rowIndex, 2].Value = owner.Id;
                    sheet.Cells[rowIndex, 3].Value = vehicle.LicensePlate;
                    sheet.Cells[rowIndex, 4].Value = vehicle.Brand;
                    sheet.Cells[rowIndex, 5].Value = vehicle.Model;
                    sheet.Cells[rowIndex, 6].Value = (int)vehicle.Color;

                    rowIndex++;
                }
            }

            await Task.CompletedTask;
        }

        private async Task CreateOrUpdateReceiptSheetAsync(ExcelPackage package,
            ObservableCollection<Receipts> receipts)
        {
            var sheet = package.Workbook.Worksheets["Receipts"] ?? package.Workbook.Worksheets.Add("Receipts");

            sheet.Cells[1, 1].Value = "Id";
            sheet.Cells[1, 2].Value = "Series";
            sheet.Cells[1, 3].Value = "Number";
            sheet.Cells[1, 4].Value = "Owner";
            sheet.Cells[1, 5].Value = "Parking";
            sheet.Cells[1, 6].Value = "ParkingLot";
            sheet.Cells[1, 7].Value = "Attendants";
            sheet.Cells[1, 8].Value = "Days";
            sheet.Cells[1, 9].Value = "Price";
            sheet.Cells[1, 10].Value = "StartDate";
            sheet.Cells[1, 11].Value = "SelectedCarId";

            int rowIndex = 2;

            for (int i = 0; i < receipts.Count; i++)
            {
                var parkingLot = receipts[i];
                sheet.Cells[i + 2, 1].Value = parkingLot.Id;
                sheet.Cells[i + 2, 2].Value = parkingLot.Series;
                sheet.Cells[i + 2, 3].Value = parkingLot.Number;
                sheet.Cells[i + 2, 4].Value = parkingLot.Owner.Id;
                sheet.Cells[i + 2, 5].Value = parkingLot.Parking.Id;
                sheet.Cells[i + 2, 6].Value = parkingLot.ParkingLot.Id;
                sheet.Cells[i + 2, 7].Value = parkingLot.Attendants.Id;
                sheet.Cells[i + 2, 8].Value = parkingLot.Days;
                sheet.Cells[i + 2, 9].Value = parkingLot.Price;
                sheet.Cells[i + 2, 10].Value =parkingLot.GetStartDate().ToString();
                sheet.Cells[i + 2, 11].Value =parkingLot.SelectedCarId.ToString();
            }

            await Task.CompletedTask;
        }

        #endregion

        #region Статистика

        /// <summary>
        /// Итог за прошедший месяц
        /// </summary>
        private async Task LastMonthsTotal(ExcelPackage package, ObservableCollection<Receipts> receipts,
            ObservableCollection<Parkings> parkings)
        {
            var lastMonth = DateTime.Now.AddMonths(-1);
            var lastMonthString = lastMonth.ToString("MM.yyyy");

            foreach (var parking in parkings)
            {
                var sheetName = parking.Name;
                var sheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == sheetName)
                            ?? package.Workbook.Worksheets.Add(sheetName);

                if (sheet.Dimension == null)
                {
                    sheet.Cells[1, 1].Value = "Id";
                    sheet.Cells[1, 2].Value = "Series";
                    sheet.Cells[1, 3].Value = "Number";
                    sheet.Cells[1, 4].Value = "Owner";
                    sheet.Cells[1, 5].Value = "Parking Lot";
                    sheet.Cells[1, 6].Value = "Days";
                    sheet.Cells[1, 7].Value = "Amount";
                    sheet.Cells[1, 8].Value = "Start Date";
                }

                var startRow = sheet.Dimension?.Rows + 1 ?? 2;

                bool totalExists = false;
                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    if (sheet.Cells[row, 6].Text.Contains($"Итог за месяц {lastMonthString}"))
                    {
                        totalExists = true;
                        break;
                    }
                }

                if (totalExists) continue;

                var lastMonthReceipts = receipts
                    .Where(r => r.Parking.Id == parking.Id &&
                                r.GetStartDate().Year == lastMonth.Year &&
                                r.GetStartDate().Month == lastMonth.Month);

                foreach (var (receipt, index) in lastMonthReceipts.Select((r, i) => (r, i)))
                {
                    sheet.Cells[startRow + index, 1].Value = receipt.Id;
                    sheet.Cells[startRow + index, 2].Value = receipt.Series;
                    sheet.Cells[startRow + index, 3].Value = receipt.Number;
                    sheet.Cells[startRow + index, 4].Value = receipt.Owner.FullNameInLine;
                    sheet.Cells[startRow + index, 5].Value = receipt.ParkingLot.Name;
                    sheet.Cells[startRow + index, 6].Value = receipt.Days;
                    sheet.Cells[startRow + index, 7].Value = receipt.Amount;
                    sheet.Cells[startRow + index, 8].Value = receipt.GetStartDate().ToShortDateString();
                }

                var totalAmount = lastMonthReceipts.Sum(r => r.Amount);

                var totalRow = startRow + lastMonthReceipts.Count();
                sheet.Cells[totalRow, 6].Value = $"Итог за месяц {lastMonthString}:";
                sheet.Cells[totalRow, 7].Value = totalAmount;

                using (var range = sheet.Cells[totalRow, 6, totalRow, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }
            }

            await Task.CompletedTask;
        }

        #endregion
    }
}
