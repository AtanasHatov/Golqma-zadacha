using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word=Microsoft.Office.Interop.Word;

namespace Task1
{
    public class Logger
    {
        private string excelFile = "log.xlsx";
        private string wordFile = "log.docx";

        public void Log(string message)
        {
            LogToExcel(message);
            LogToWord(message);
        }

        private void LogToExcel(string message)
        {
            using (ExcelPackage package = new ExcelPackage(new FileInfo(excelFile)))
            {
                var sheet = package.Workbook.Worksheets.Count > 0 ? package.Workbook.Worksheets[0] : package.Workbook.Worksheets.Add("Log");
                int row = sheet.Dimension?.Rows + 1 ?? 1;
                sheet.Cells[row, 1].Value = DateTime.Now;
                sheet.Cells[row, 2].Value = message;
                package.Save();
            }
        }

        private void LogToWord(string message)
        {
            Word.Application wordApp = new Word.Application();
            Word.Document doc = File.Exists(wordFile) ? wordApp.Documents.Open(Path.GetFullPath(wordFile)) : wordApp.Documents.Add();
            doc.Content.Text += $"{DateTime.Now} - {message}\n";
            doc.SaveAs2(wordFile);
            doc.Close();
            wordApp.Quit();
        }
    }
}
