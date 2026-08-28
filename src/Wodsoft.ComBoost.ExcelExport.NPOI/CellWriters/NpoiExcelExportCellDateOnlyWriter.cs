#if NET6_0_OR_GREATER
using NPOI.SS.UserModel;
using System;

namespace Wodsoft.ComBoost.ExcelExport.NPOI.CellWriters
{
    /// <summary>
    /// Writes <see cref="DateOnly"/> values to NPOI cells.
    /// </summary>
    public class NpoiExcelExportCellDateOnlyWriter : INpoiExcelExportCellWriter
    {
        /// <inheritdoc />
        public bool CanWrite(Type type) => type == typeof(DateOnly);

        /// <inheritdoc />
        public void Write<TExport>(NpoiExcelExportService service, NpoiExcelExportContext context, IExcelExportColumn<TExport> column, ICell cell, TExport item)
        {
            if (column.Type != typeof(DateOnly) && column.Type != typeof(DateOnly?))
                throw new InvalidOperationException("Invalid type of column.");
            DateOnly value;
            if (column is IExcelExportColumn<TExport, DateOnly> dateOnlyColumn)
            {
                value = dateOnlyColumn.Read(item);
            }
            else if (column is IExcelExportColumn<TExport, DateOnly?> nullableDateOnlyColumn)
            {
                var nullableValue = nullableDateOnlyColumn.Read(item);
                if (nullableValue.HasValue == false)
                    return;
                value = nullableValue.Value;
            }
            else
            {
                throw new ArgumentException($"Export column does not implement \"{typeof(IExcelExportColumn<TExport, DateOnly>).FullName}\" or \"{typeof(IExcelExportColumn<TExport, DateOnly?>).FullName}\".");
            }
            service.WriteCell(context, cell, column, item, value.ToDateTime(TimeOnly.MinValue));
        }
    }
}
#endif
