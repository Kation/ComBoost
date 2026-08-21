using NPOI.SS.UserModel;
using System;

namespace Wodsoft.ComBoost.ExcelExport.NPOI.CellWriters
{
    /// <summary>
    /// Writes <see cref="DateTime"/> values to NPOI cells.
    /// </summary>
    public class NpoiExcelExportCellDateTimeOffsetWriter : INpoiExcelExportCellWriter
    {
        /// <inheritdoc />
        public bool CanWrite(Type type) => type == typeof(DateTime);

        /// <inheritdoc />
        public void Write<TExport>(IExcelExportColumn<TExport> column, ICell cell, TExport item)
        {
            if (column.Type != typeof(DateTimeOffset) && column.Type != typeof(DateTimeOffset?))
                throw new InvalidOperationException("Invalid type of column.");
            DateTimeOffset value;
            if (column is IExcelExportColumn<TExport, DateTimeOffset> dateTimeColumn)
            {
                value = dateTimeColumn.Read(item);
            }
            else if (column is IExcelExportColumn<TExport, DateTimeOffset?> nullableDateTimeColumn)
            {
                var nullableValue = nullableDateTimeColumn.Read(item);
                if (nullableValue.HasValue == false)
                    return;
                value = nullableValue.Value;
            }
            else
            {
                throw new ArgumentException($"Export column does not implement \"{typeof(IExcelExportColumn<TExport, DateTimeOffset>).FullName}\" or \"{typeof(IExcelExportColumn<TExport, DateTimeOffset?>).FullName}\".");
            }
            cell.SetCellValue(value.LocalDateTime);
        }
    }
}
