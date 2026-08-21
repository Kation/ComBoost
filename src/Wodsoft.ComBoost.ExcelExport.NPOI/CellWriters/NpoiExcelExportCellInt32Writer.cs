using NPOI.SS.UserModel;
using System;

namespace Wodsoft.ComBoost.ExcelExport.NPOI.CellWriters
{
    /// <summary>
    /// Writes <see cref="int"/> values to NPOI cells.
    /// </summary>
    public class NpoiExcelExportCellInt32Writer : INpoiExcelExportCellWriter
    {
        /// <inheritdoc />
        public bool CanWrite(Type type) => type == typeof(int);

        /// <inheritdoc />
        public void Write<TExport>(IExcelExportColumn<TExport> column, ICell cell, TExport item)
        {
            if (column.Type != typeof(int) && column.Type != typeof(int?))
                throw new InvalidOperationException("Invalid type of column.");
            int value;
            if (column is IExcelExportColumn<TExport, int> int32Column)
            {
                value = int32Column.Read(item);
            }
            else if (column is IExcelExportColumn<TExport, int?> nullableInt32Column)
            {
                var nullableValue = nullableInt32Column.Read(item);
                if (nullableValue.HasValue == false)
                    return;
                value = nullableValue.Value;
            }
            else
            {
                throw new ArgumentException($"Export column does not implement \"{typeof(IExcelExportColumn<TExport, int>).FullName}\" or \"{typeof(IExcelExportColumn<TExport, int?>).FullName}\".");
            }
            cell.SetCellValue(value);
        }
    }
}
