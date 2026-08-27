using NPOI.SS.UserModel;
using System;

namespace Wodsoft.ComBoost.ExcelExport.NPOI.CellWriters
{
    /// <summary>
    /// Writes <see cref="DateTime"/> values to NPOI cells.
    /// </summary>
    public class NpoiExcelExportCellDateTimeWriter : INpoiExcelExportCellWriter
    {
        /// <inheritdoc />
        public bool CanWrite(Type type) => type == typeof(DateTime);

        /// <inheritdoc />
        public void Write<TExport>(NpoiExcelExportService service, NpoiExcelExportContext context, IExcelExportColumn<TExport> column, ICell cell, TExport item)
        {
            if (column.Type != typeof(DateTime) && column.Type != typeof(DateTime?))
                throw new InvalidOperationException("Invalid type of column.");
            DateTime value;
            if (column is IExcelExportColumn<TExport, DateTime> dateTimeColumn)
            {
                value = dateTimeColumn.Read(item);
            }
            else if (column is IExcelExportColumn<TExport, DateTime?> nullableDateTimeColumn)
            {
                var nullableValue = nullableDateTimeColumn.Read(item);
                if (nullableValue.HasValue == false)
                    return;
                value = nullableValue.Value;
            }
            else
            {
                throw new ArgumentException($"Export column does not implement \"{typeof(IExcelExportColumn<TExport, DateTime>).FullName}\" or \"{typeof(IExcelExportColumn<TExport, DateTime?>).FullName}\".");
            }
            service.WriteCell(context, cell, column, item, value);
        }
    }
}
