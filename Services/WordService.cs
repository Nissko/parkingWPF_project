using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Win32;
using ParkingWork.Entities.Parking.Receipt;
using Word = Microsoft.Office.Interop.Word;

namespace ParkingWork.Services
{
    public class WordService
    {
        private readonly string _templatePath = @"C:\GitHubRepositories\parkingWPF_project\ReceiptTemplate\Template.docx";
        private readonly string _outputPath = @"C:\GitHubRepositories\parkingWPF_project\ReceiptTemplate\cache\cache.docx";

        public string GenerateReceipt(Dictionary<string, string> replacements)
        {
            //TODO: Exception
            if (replacements == null || !replacements.Any())
                throw new ArgumentException("Словарь замен не может быть пустым.", nameof(replacements));

            try
            {
                File.Copy(_templatePath, _outputPath, true);

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(_outputPath, true))
                {
                    var body = wordDoc.MainDocumentPart?.Document.Body;

                    //TODO: Exception
                    if (body == null)
                        throw new InvalidOperationException("Шаблон пустой или не содержит тела документа.");

                    foreach (var element in body.Elements())
                    {
                        if (element is Paragraph paragraph)
                        {
                            ReplaceTextInParagraph(paragraph, replacements);
                        }
                        else if (element is Table table)
                        {
                            ReplaceTextInTable(table, replacements);
                        }
                    }

                    wordDoc.MainDocumentPart.Document.Save();
                }

                return _outputPath;
            }
            catch (Exception ex)
            {
                //TODO: Exception
                Console.WriteLine($"Ошибка при обработке документа: {ex.Message}");
                return string.Empty;
            }
        }
        
        public string ConvertWordToPdf(string wordCacheFilePath)
        {
            var wordApp = new Word.Application();
            Word.Document document = null;
            try
            {
                document = wordApp.Documents.Open(wordCacheFilePath);
                string pdfFilePath = Path.ChangeExtension(wordCacheFilePath, ".pdf");
                document.ExportAsFixedFormat(pdfFilePath, Word.WdExportFormat.wdExportFormatPDF);
                return pdfFilePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                return string.Empty;
            }
            finally
            {
                if (document != null)
                {
                    document.Close(false);
                    Marshal.ReleaseComObject(document);
                }
                wordApp.Quit();
                Marshal.ReleaseComObject(wordApp);
            }
        }

        /// <summary>
        /// Сохранение всех квитанций в 1 файл
        /// </summary>
        public void SaveAllReceiptsToSingleWordFile(ObservableCollection<Receipts> receipts)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Сохранить все квитанции",
                Filter = "Word файлы (*.docx)|*.docx",
                FileName = $"Квитанции_{DateTime.Now:dd-MM-yyyy}.docx"
            };

            if (saveFileDialog.ShowDialog() == true)
                try
                {
                    var filePath = saveFileDialog.FileName;

                    var tempOutputPath = Path.GetTempFileName();
                    File.Copy(_templatePath, tempOutputPath, true);

                    using (var wordDoc = WordprocessingDocument.Open(tempOutputPath, true))
                    {
                        var mainDocumentPart = wordDoc.MainDocumentPart;
                        if (mainDocumentPart == null)
                            throw new Exception("Ошибка при создании основного раздела документа.");

                        var body = mainDocumentPart.Document.Body;
                        if (body == null) throw new Exception("Шаблон пустой");

                        var templateBody = body.CloneNode(true);

                        body.RemoveAllChildren();

                        foreach (var receipt in receipts)
                        {
                            var receiptBody = templateBody.CloneNode(true);

                            var tags = new Dictionary<string, string>
                            {
                                { "<PARKINGNAME>", receipt.Parking.Name },
                                { "<INNPARKING>", $"ИНН: {receipt.Parking.Inn}" },
                                { "<ADDRESSPARKING>", receipt.Parking.Address },
                                { "<SERIES>", receipt.Series },
                                { "<NUMBER>", receipt.Number },
                                { "<PARKINGLOT>", receipt.ParkingLot.Name },

                                {
                                    "<CARBRAND>",
                                    receipt.Owner.Vehicles.FirstOrDefault(t => t.Id == receipt.SelectedCarId)?.Brand
                                },
                                {
                                    "<CARMODEL>",
                                    receipt.Owner.Vehicles.FirstOrDefault(t => t.Id == receipt.SelectedCarId)?.Model
                                },
                                {
                                    "<LICENSEPLATE>",
                                    receipt.Owner.Vehicles.FirstOrDefault(t => t.Id == receipt.SelectedCarId)
                                        ?.LicensePlate
                                },

                                { "<FIOOWNER>", receipt.Owner.FullNameInLine },
                                { "<ADDRESSOWNER>", receipt.Owner.Address },
                                { "<PHONEOWNER>", receipt.Owner.Phone },

                                { "<STARTDATE>", receipt.GetStartDate().ToShortDateString() },
                                { "<SHOUR>", receipt.GetStartDate().ToString("HH") },
                                { "<SMINE>", receipt.GetStartDate().ToString("mm") },
                                { "<STARTPARK>", receipt.GetStartDate().ToString("dd.MM.yyyy") },
                                { "<ENDPARK>", receipt.EndDate.ToString("dd.MM.yyyy") },

                                { "<FIOATTENDANT>", receipt.Attendants.FullNameInLine },

                                { "<DAYS>", receipt.Days.ToString() },
                                { "<AMOUNT>", receipt.Amount.ToString() }
                            };

                            foreach (var element in ((Body)receiptBody).Descendants())
                            {
                                if (element is Paragraph paragraph) ReplaceTextInParagraph(paragraph, tags);

                                if (element is Table table) ReplaceTextInTable(table, tags);
                            }

                            body.AppendChild(receiptBody);

                            if (receipt != receipts.Last())
                                body.AppendChild(new Paragraph(new Run(new Break { Type = BreakValues.Page })));
                        }

                        mainDocumentPart.Document.Save();
                    }

                    File.Copy(tempOutputPath, filePath, true);

                    MessageBox.Show("Все квитанции успешно сохранены в один Word файл!", "Успех", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
        }
        
        /// <summary>
        /// Метод для замены текста в абзаце
        /// </summary>
        private void ReplaceTextInParagraph(Paragraph paragraph, Dictionary<string, string> replacements)
        {
            //TODO: Exception
            if (paragraph == null || replacements == null || !replacements.Any())
                return;

            foreach (var run in paragraph.Elements<Run>())
            {
                var textElement = run.GetFirstChild<Text>();
                if (textElement == null || string.IsNullOrEmpty(textElement.Text))
                    continue;

                foreach (var replacement in replacements)
                {
                    if (textElement.Text.Contains(replacement.Key))
                    {
                        textElement.Text = textElement.Text.Replace(replacement.Key, replacement.Value);
                    }
                }
            }
        }
        
        /// <summary>
        /// Метод для замены текста в таблице
        /// </summary>
        private void ReplaceTextInTable(Table table, Dictionary<string, string> replacements)
        {
            //TODO: Exception
            if (table == null || replacements == null || !replacements.Any())
                return;

            foreach (var row in table.Elements<TableRow>())
            {
                foreach (var cell in row.Elements<TableCell>())
                {
                    foreach (var paragraph in cell.Elements<Paragraph>())
                    {
                        ReplaceTextInParagraph(paragraph, replacements);
                    }
                }
            }
        }
    }
}
