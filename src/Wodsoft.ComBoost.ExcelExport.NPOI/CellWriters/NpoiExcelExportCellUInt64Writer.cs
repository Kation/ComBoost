using NPOI.SS.UserModel;
using System;

namespace Wodsoft.ComBoost.ExcelExport.NPOI.CellWriters
{
    /// <summary>
    /// Writes <see cref="ulong"/> values to NPOI cells.
    /// </summary>
    public class NpoiExcelExportCellUInt64Writer : INpoiExcelExportCellWriter
    {
        /// <inheritdoc />
        public bool CanWrite(Type type) => type == typeof(ulong);

        /// <inheritdoc />
        public void Write<TExport>(IExcelExportColumn<TExport> column, ICell cell, TExport item)
        {
            if (column.Type != typeof(ulong) && column.Type != typeof(ulong?))
                throw new InvalidOperationException("Invalid type of column.");
            ulong value;
            if (column is IExcelExportColumn<TExport, ulong> uint64Column)
            {
                value = uint64Column.Read(item);
            }
            else if (column is IExcelExportColumn<TExport, ulong?> nullableUInt64Column)
            {
                var nullableValue = nullableUInt64Column.Read(item);
                if (nullableValue.HasValue == false)
                    return;
                value = nullableValue.Value;
            }
            else
            {
                throw new ArgumentException($"Export column does not implement \"{typeof(IExcelExportColumn<TExport, ulong>).FullName}\" or \"{typeof(IExcelExportColumn<TExport, ulong?>).FullName}\".");
            }
            // Excel numeric cells are double-based, so large UInt64 values may lose precision.
            cell.SetCellValue((double)value);
        }
    }
}
