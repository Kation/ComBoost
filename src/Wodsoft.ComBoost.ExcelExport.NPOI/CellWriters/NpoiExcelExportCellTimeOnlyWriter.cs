#if NET6_0_OR_GREATER
using NPOI.SS.UserModel;
using System;

namespace Wodsoft.ComBoost.ExcelExport.NPOI.CellWriters
{
    /// <summary>
    /// Writes <see cref="TimeOnly"/> values to NPOI cells as an Excel time serial, a fraction of a day.
    /// </summary>
    public class NpoiExcelExportCellTimeOnlyWriter : INpoiExcelExportCellWriter
    {
        /// <inheritdoc />
        public bool CanWrite(Type type) => type == typeof(TimeOnly);

        /// <inheritdoc />
        public void Write<TExport>(NpoiExcelExportService service, NpoiExcelExportContext context, IExcelExportColumn<TExport> column, ICell cell, TExport item)
        {
            if (column.Type != typeof(TimeOnly) && column.Type != typeof(TimeOnly?))
                throw new InvalidOperationException("Invalid type of column.");
            TimeOnly value;
            if (column is IExcelExportColumn<TExport, TimeOnly> timeOnlyColumn)
            {
                value = timeOnlyColumn.Read(item);
            }
            else if (column is IExcelExportColumn<TExport, TimeOnly?> nullableTimeOnlyColumn)
            {
                var nullableValue = nullableTimeOnlyColumn.Read(item);
                if (nullableValue.HasValue == false)
                    return;
                value = nullableValue.Value;
            }
            else
            {
                throw new ArgumentException($"Export column does not implement \"{typeof(IExcelExportColumn<TExport, TimeOnly>).FullName}\" or \"{typeof(IExcelExportColumn<TExport, TimeOnly?>).FullName}\".");
            }
            service.WriteCell(context, cell, column, item, value.ToTimeSpan().TotalDays);
        }
    }
}
#endif
