using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Word = Microsoft.Office.Interop.Word;

namespace ParkingWork.Services
{
    public class WordService
    {
        private readonly string _templatePath = @"C:\GitHubRepositories\parkingWPF_project\ReceiptTemplate\Template.docx";
        private readonly string _outputPath = @"C:\GitHubRepositories\parkingWPF_project\ReceiptTemplate\cache\cache.docx";

        public string GenerateReceipt(Dictionary<string, string> replacements)
        {
            try
            {
                File.Copy(_templatePath, _outputPath, true);

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(_outputPath, true))
                {
                    var body = wordDoc.MainDocumentPart.Document.Body;

                    if (body == null)
                        throw new Exception("Шаблон пустой");

                    foreach (var element in body.Descendants())
                    {
                        if (element is Paragraph _paragraph)
                        {
                            ReplaceTextInParagraph(_paragraph, replacements);
                        }
                        
                        if (element is Table table)
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
        /// Метод для замены текста в абзаце
        /// </summary>
        private void ReplaceTextInParagraph(Paragraph paragraph, Dictionary<string, string> replacements)
        {
            foreach (var run in paragraph.Descendants<Run>())
            {
                var textElement = run.GetFirstChild<Text>();
                if (textElement == null) continue;

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
            foreach (var row in table.Descendants<TableRow>())
            {
                foreach (var cell in row.Descendants<TableCell>())
                {
                    foreach (var paragraph in cell.Descendants<Paragraph>())
                    {
                        ReplaceTextInParagraph(paragraph, replacements);
                    }
                }
            }
        }
    }
}
