using ClosedXML.Excel;
using eXtensionSharp;

namespace Jina.Excel
{
    public class ExcelWrapper
    {
        private readonly ExcelWrapperCore _excelWrapperCore;
        public ExcelWrapper()
        {
            _excelWrapperCore = new ExcelWrapperCore();
        }

        /// <summary>
        /// Create excel
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="sheetName"></param>
        /// <param name="datum"></param>
        /// <param name="worksheetHandler">after create, modify style, mergem etc</param>
        public void CreateExcel(string filePath, string sheetName, List<DynamicDictionary<string>> datum, Action<IXLWorksheet> worksheetHandler = null)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add(sheetName);

            _excelWrapperCore.CreateExcelCore(ws, datum, worksheetHandler);

            workbook.SaveAs(filePath);
        }

        /// <summary>
        /// Create excel to byte array
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="datum"></param>
        /// <param name="worksheetHandler">after create, modify style, mergem etc</param>
        /// <returns></returns>
        public byte[] CreateExcel(string sheetName, List<DynamicDictionary<string>> datum, Action<IXLWorksheet> worksheetHandler = null)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add(sheetName);

            _excelWrapperCore.CreateExcelCore(ws, datum, worksheetHandler);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
        
        /// <summary>
        /// Manually crate excel
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="sheetName"></param>
        /// <param name="worksheetHandler"></param>
        public void CreateExcel(string filePath, string sheetName, Action<IXLWorksheet> worksheetHandler)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add(sheetName);

            if(worksheetHandler.xIsNotEmpty()) 
            {
                worksheetHandler(ws);
            }

            workbook.SaveAs(filePath);
        }
    }
}
