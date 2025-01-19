using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using OfficeOpenXml;
using ParkingWork.Entities.Attendants;
using ParkingWork.Entities.Owner;
using ParkingWork.Entities.Parking;

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

        public async Task SaveDataToExcelAsync(ObservableCollection<Owners> owners, ObservableCollection<Parkings> parkings,
            ObservableCollection<ParkingLots> parkingLots, ObservableCollection<Attendants> attendants)
        {
            FileInfo newFile = new FileInfo(_filePath);
            using (var package = new ExcelPackage(newFile))
            {
                await CreateOrUpdateOwnersSheetAsync(package, owners);
                await CreateOrUpdateParkingsSheetAsync(package, parkings);
                await CreateOrUpdateParkingLotsSheetAsync(package, parkingLots);
                await CreateOrUpdateAttendantsSheetAsync(package, attendants);
                await CreateOrUpdateVehiclesSheetAsync(package, owners);

                await package.SaveAsync();
            }
        }

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

            for (int i = 0; i < parkings.Count; i++)
            {
                var parking = parkings[i];
                sheet.Cells[i + 2, 1].Value = parking.Id;
                sheet.Cells[i + 2, 2].Value = parking.Name;
                sheet.Cells[i + 2, 3].Value = parking.Address;
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
    }
}
