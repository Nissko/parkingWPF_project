using ParkingWork.Entities.Attendants;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using OfficeOpenXml;
using ParkingWork.Entities.Owner;
using ParkingWork.Entities.Parking;
using ParkingWork.Entities.Parking.Receipt;
using ParkingWork.Entities.Vehicle;
using ParkingWork.Entities.Vehicle.Enum;

namespace ParkingWork.Services
{
    public class ExcelDataLoaderService
    {
        #region Attendants

        public async Task<List<Attendants>> LoadAttendantsFromExcel(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            var attendants = new List<Attendants>();

            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var listName = "Attendants";
                    
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == listName);

                    if (worksheet == null)
                    {
                        MessageBox.Show($"Лист с именем '{listName}' не найден", "Ошибка");
                        return attendants;
                    }
                    
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var id = worksheet.Cells[row, 1].Text; // Идентификатор
                        var name = worksheet.Cells[row, 2].Text; // Имя
                        var surname = worksheet.Cells[row, 3].Text; // Фамилия
                        var patronymic = worksheet.Cells[row, 4].Text; // Отчество

                        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname) ||
                            string.IsNullOrEmpty(patronymic)) 
                            continue;

                        var attendant = new Attendants(Guid.Parse(id), name, surname, patronymic);

                        attendants.Add(attendant);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных из Excel: {ex.Message}",$"Ошибка");
            }

            return attendants;
        }

        #endregion

        #region Owners

        public async Task<List<Owners>> LoadOwnersFromExcel(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            var owners = new List<Owners>();

            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var listName = "Owners";
                    
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == listName);

                    if (worksheet == null)
                    {
                        MessageBox.Show($"Лист с именем '{listName}' не найден", "Ошибка");
                        return owners;
                    }
                    
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var id = worksheet.Cells[row, 1].Text; // Идентификатор
                        var name = worksheet.Cells[row, 2].Text; // Имя
                        var surname = worksheet.Cells[row, 3].Text; // Фамилия
                        var patronymic = worksheet.Cells[row, 4].Text; // Отчество
                        var address = worksheet.Cells[row, 5].Text; // Адрес
                        var phone = worksheet.Cells[row, 6].Value.ToString(); // Телефон
                        
                        var cars = await GetAllOwnersVehicles(Guid.Parse(id), filePath);

                        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname) ||
                            string.IsNullOrEmpty(patronymic) || string.IsNullOrEmpty(address) ||
                            string.IsNullOrEmpty(phone) || cars.Count == 0)
                            continue;
                        
                        var owner = new Owners(id: Guid.Parse(id), name: name, surname: surname, patronymic: patronymic,
                            address: address, phone: phone, vehicles: cars);

                        owners.Add(owner);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных из Excel: {ex.Message}",$"Ошибка");
            }

            return owners;
        }

        /// <summary>
        /// Получение всех авто клиентов
        /// </summary>
        public async Task<List<Vehicles>> GetAllOwnersVehicles(Guid clientId, string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            var carsExcel = new List<Vehicles>();
            
            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var listName = "Vehicles";
                    
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == listName);

                    if (worksheet == null)
                    {
                        MessageBox.Show($"Лист с именем '{listName}' не найден", "Ошибка");
                        return carsExcel;
                    }
                    
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var clientExcelId = worksheet.Cells[row, 2].Text;

                        if (Guid.Parse(clientExcelId) != clientId) continue;

                        var id = worksheet.Cells[row, 1].Text;
                        var licensePlate = worksheet.Cells[row, 3].Text; // Номер
                        var brand = worksheet.Cells[row, 4].Text; // Марка авто
                        var model = worksheet.Cells[row, 5].Text; // Модель авто
                        var color = worksheet.Cells[row, 6].Text; // Цвет

                        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(licensePlate) ||
                            string.IsNullOrEmpty(brand) || string.IsNullOrEmpty(model) || string.IsNullOrEmpty(color)) 
                        {
                            continue;
                        }

                        var intColor = int.Parse(color);
                        var colorEnum = (VehicleColorEnums)intColor;
                        
                        var car = new Vehicles(Guid.Parse(id), clientId, licensePlate, brand, model, colorEnum);
                        carsExcel.Add(car);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных из Excel: {ex.Message}",$"Ошибка");
            }

            return carsExcel;
        }

        #endregion

        #region ParkingLots

        public async Task<List<ParkingLots>> LoadParkingLotsFromExcel(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            var parkingLots = new List<ParkingLots>();

            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var listName = "ParkingLots";
                    
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == listName);

                    if (worksheet == null)
                    {
                        MessageBox.Show($"Лист с именем '{listName}' не найден", "Ошибка");
                        return parkingLots;
                    }
                    
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var id = worksheet.Cells[row, 1].Text; // Идентификатор
                        var parkingId = worksheet.Cells[row, 2].Text; // Идентификатор парковки
                        var name = worksheet.Cells[row, 3].Text; // Название места
                        var isFree = worksheet.Cells[row, 4].Text; // Свободно место или нет
                        
                        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(parkingId) ||
                            string.IsNullOrEmpty(name) || string.IsNullOrEmpty(isFree)) 
                            continue;
                        
                        bool isTrueFalse = isFree == "1" ? true : false;

                        var parking = await GetParkingName(Guid.Parse(parkingId), filePath);

                        var parkingLot = new ParkingLots(Guid.Parse(id), Guid.Parse(parkingId), name,
                            parkingName: parking, isTrueFalse);

                        parkingLots.Add(parkingLot);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных из Excel: {ex.Message}",$"Ошибка");
            }

            return parkingLots;
        }

        public async Task<string> GetParkingName(Guid parkingId, string filePath)
        {
            var parkingName = string.Empty;
            
            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var listName = "Parking";
                    
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == listName);

                    if (worksheet == null)
                    {
                        MessageBox.Show($"Лист с именем '{listName}' не найден", "Ошибка");
                        return parkingName;
                    }
                    
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var id = worksheet.Cells[row, 1].Text; // Идентификатор
                        var name = worksheet.Cells[row, 2].Text; // Название стоянки

                        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name))
                            continue;
                        
                        if (Guid.Parse(id) == parkingId)
                            return name;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных из Excel: {ex.Message}",$"Ошибка");
            }

            return parkingName;
        }

        #endregion

        #region Parkings

        public async Task<List<Parkings>> LoadParkingFromExcel(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            var parkings = new List<Parkings>();

            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var listName = "Parking";
                    
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == listName);

                    if (worksheet == null)
                    {
                        MessageBox.Show($"Лист с именем '{listName}' не найден", "Ошибка");
                        return parkings;
                    }
                    
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var id = worksheet.Cells[row, 1].Text; // Идентификатор
                        var name = worksheet.Cells[row, 2].Text; // Название стоянки
                        var address = worksheet.Cells[row, 3].Text; // Адрес
                        var inn = worksheet.Cells[row, 4].Text; // Инн
                        
                        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(inn))
                            continue;

                        var parking = new Parkings(Guid.Parse(id), name, address, BigInteger.Parse(inn));

                        parkings.Add(parking);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных из Excel: {ex.Message}",$"Ошибка");
            }

            return parkings;
        }

        #endregion

        #region Vehicles

        public async Task<List<Vehicles>> LoadVehicleFromExcel(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            var vehicles = new List<Vehicles>();

            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var listName = "Vehicles";
                    
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == listName);

                    if (worksheet == null)
                    {
                        MessageBox.Show($"Лист с именем '{listName}' не найден", "Ошибка");
                        return vehicles;
                    }
                    
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var id = worksheet.Cells[row, 1].Text; // Идентификатор
                        var clientId = worksheet.Cells[row, 2].Text; // Клиент
                        var licensePlate = worksheet.Cells[row, 3].Text; // Номер авто
                        var brand = worksheet.Cells[row, 4].Text; // Бренд авто
                        var model = worksheet.Cells[row, 5].Text; // Модель авто
                        var color = worksheet.Cells[row, 6].Text; // Цвет авто

                        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(clientId) ||
                            string.IsNullOrEmpty(licensePlate) || string.IsNullOrEmpty(brand) ||
                            string.IsNullOrEmpty(model) || string.IsNullOrEmpty(color)) 
                            continue;

                        var intColor = int.Parse(color);
                        var colorEnum = (VehicleColorEnums)intColor;

                        var vehicle = new Vehicles(Guid.Parse(id), Guid.Parse(clientId), licensePlate, brand, model,
                            colorEnum);

                        vehicles.Add(vehicle);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных из Excel: {ex.Message}",$"Ошибка");
            }

            return vehicles;
        }

        #endregion

        #region Receipts

        public async Task<List<Receipts>> LoadReceiptFromExcel(string filePath, ObservableCollection<Owners> ownersList,
            ObservableCollection<Parkings> parkingList, ObservableCollection<ParkingLots> parkingLotsList,
            ObservableCollection<Attendants> attendantsList)

        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var receipts = new List<Receipts>();

            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var listName = "Receipts";

                    var worksheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == listName);

                    if (worksheet == null)
                    {
                        MessageBox.Show($"Лист с именем '{listName}' не найден", "Ошибка");
                        return receipts;
                    }

                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var id = worksheet.Cells[row, 1].Text;
                        var series = worksheet.Cells[row, 2].Text;
                        var number = worksheet.Cells[row, 3].Text;
                        var ownerId = worksheet.Cells[row, 4].Text;
                        var parkingId = worksheet.Cells[row, 5].Text;
                        var parkinglotId = worksheet.Cells[row, 6].Text;
                        var attendantsId = worksheet.Cells[row, 7].Text;
                        var days = worksheet.Cells[row, 8].Text;
                        var price = worksheet.Cells[row, 9].Text;
                        var startdate = worksheet.Cells[row, 10].Text;
                        var selectedcarid = worksheet.Cells[row, 11].Text;

                        if (string.IsNullOrEmpty(id) ||
                            string.IsNullOrEmpty(series) ||
                            string.IsNullOrEmpty(number) ||
                            string.IsNullOrEmpty(ownerId) ||
                            string.IsNullOrEmpty(parkingId) ||
                            string.IsNullOrEmpty(parkinglotId) ||
                            string.IsNullOrEmpty(attendantsId) ||
                            string.IsNullOrEmpty(days) ||
                            string.IsNullOrEmpty(price) ||
                            string.IsNullOrEmpty(startdate) ||
                            string.IsNullOrEmpty(selectedcarid))
                        {
                            continue;
                        }
                        
                        var ownerEntity = ownersList.FirstOrDefault(o => o.Id == Guid.Parse(ownerId));
                        var parkingEntity = parkingList.FirstOrDefault(p => p.Id == Guid.Parse(parkingId));
                        var parkingLotEntity = parkingLotsList.FirstOrDefault(p => p.Id == Guid.Parse(parkinglotId));
                        var attendantsEntity = attendantsList.FirstOrDefault(p => p.Id == Guid.Parse(attendantsId));

                        var receipt = new Receipts(Guid.Parse(id), series, number, ownerEntity, parkingEntity,
                            parkingLotEntity, attendantsEntity, int.Parse(days), decimal.Parse(price),
                            Guid.Parse(selectedcarid), DateTime.Parse(startdate));

                        receipts.Add(receipt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных из Excel: {ex.Message}", $"Ошибка");
            }

            return receipts;
        }

        #endregion
    }
}
