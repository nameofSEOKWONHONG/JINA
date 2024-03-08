using ClosedXML.Excel;
using eXtensionSharp;

namespace Jina.Excel
{
    internal class ExcelWrapperCore
    {
        public ExcelWrapperCore()
        {
            
        }

        public void CreateExcelCore(IXLWorksheet ws, List<DynamicDictionary<string>> datum, Action<IXLWorksheet> worksheetHandler = null)
        {
            var rowIdx = 1;
            var headers = datum.xFirst().Keys.ToArray();
            for (var i = 0; i < headers.Length; i++)
            {
                var header = headers[i];
                ws.Cell(rowIdx, (i + 1)).Value = header;
            }
            rowIdx++;

            foreach (var data in datum)
            {
                var colIdx = 1;
                foreach (var header in headers)
                {
                    ws.Cell(rowIdx, colIdx).Value = data[header];
                    colIdx++;
                }
                rowIdx++;
            }

            if (worksheetHandler.xIsNotEmpty())
            {
                worksheetHandler(ws);
            }
        }
    }
}
