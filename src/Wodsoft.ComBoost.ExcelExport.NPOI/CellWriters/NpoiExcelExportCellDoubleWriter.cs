using NPOI.SS.UserModel;
using System;

namespace Wodsoft.ComBoost.ExcelExport.NPOI.CellWriters
{
    /// <summary>
    /// Writes <see cref="double"/> values to NPOI cells.
    /// </summary>
    public class NpoiExcelExportCellDoubleWriter : INpoiExcelExportCellWriter
    {
        /// <inheritdoc />
        public bool CanWrite(Type type) => type == typeof(double);

        /// <inheritdoc />
        public void Write<TExport>(IExcelExportColumn<TExport> column, ICell cell, TExport item)
        {
            if (column.Type != typeof(double) && column.Type != typeof(double?))
                throw new InvalidOperationException("Invalid type of column.");
            double value;
            if (column is IExcelExportColumn<TExport, double> doubleColumn)
            {
                value = doubleColumn.Read(item);
            }
            else if (column is IExcelExportColumn<TExport, double?> nullableDoubleColumn)
            {
                var nullableValue = nullableDoubleColumn.Read(item);
                if (nullableValue.HasValue == false)
                    return;
                value = nullableValue.Value;
            }
            else
            {
                throw new ArgumentException($"Export column does not implement \"{typeof(IExcelExportColumn<TExport, double>).FullName}\" or \"{typeof(IExcelExportColumn<TExport, double?>).FullName}\".");
            }
            cell.SetCellValue(value);
        }
    }
}
