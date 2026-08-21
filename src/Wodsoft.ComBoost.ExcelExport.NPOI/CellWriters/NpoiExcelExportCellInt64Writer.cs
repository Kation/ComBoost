using NPOI.SS.UserModel;
using System;

namespace Wodsoft.ComBoost.ExcelExport.NPOI.CellWriters
{
    /// <summary>
    /// Writes <see cref="long"/> values to NPOI cells.
    /// </summary>
    public class NpoiExcelExportCellInt64Writer : INpoiExcelExportCellWriter
    {
        /// <inheritdoc />
        public bool CanWrite(Type type) => type == typeof(long);

        /// <inheritdoc />
        public void Write<TExport>(IExcelExportColumn<TExport> column, ICell cell, TExport item)
        {
            if (column.Type != typeof(long) && column.Type != typeof(long?))
                throw new InvalidOperationException("Invalid type of column.");
            long value;
            if (column is IExcelExportColumn<TExport, long> int64Column)
            {
                value = int64Column.Read(item);
            }
            else if (column is IExcelExportColumn<TExport, long?> nullableInt64Column)
            {
                var nullableValue = nullableInt64Column.Read(item);
                if (nullableValue.HasValue == false)
                    return;
                value = nullableValue.Value;
            }
            else
            {
                throw new ArgumentException($"Export column does not implement \"{typeof(IExcelExportColumn<TExport, long>).FullName}\" or \"{typeof(IExcelExportColumn<TExport, long?>).FullName}\".");
            }
            cell.SetCellValue(value);
        }
    }
}
