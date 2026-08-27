using NPOI.SS.UserModel;
using System;

namespace Wodsoft.ComBoost.ExcelExport.NPOI.CellWriters
{
    /// <summary>
    /// Writes <see cref="string"/> values to NPOI cells.
    /// </summary>
    public class NpoiExcelExportCellStringWriter : INpoiExcelExportCellWriter
    {
        /// <inheritdoc />
        public bool CanWrite(Type type) => type == typeof(string);

        /// <inheritdoc />
        public void Write<TExport>(NpoiExcelExportService service, NpoiExcelExportContext context, IExcelExportColumn<TExport> column, ICell cell, TExport item)
        {
            if (column.Type != typeof(string))
                throw new InvalidOperationException("Invalid type of column.");
            if (column is not IExcelExportColumn<TExport, string> columnReader)
                throw new ArgumentException($"Export column does not implement \"{typeof(IExcelExportColumn<TExport, string>).FullName}\".");
            var value = columnReader.Read(item);
            if (value is null)
                return;
            service.WriteCell(context, cell, column, item, value);
        }
    }
}
