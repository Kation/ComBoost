using NPOI.SS.UserModel;
using System;

namespace Wodsoft.ComBoost.ExcelExport.NPOI.CellWriters
{
    /// <summary>
    /// Writes <see cref="float"/> values to NPOI cells.
    /// </summary>
    public class NpoiExcelExportCellSingleWriter : INpoiExcelExportCellWriter
    {
        /// <inheritdoc />
        public bool CanWrite(Type type) => type == typeof(float);

        /// <inheritdoc />
        public void Write<TExport>(IExcelExportColumn<TExport> column, ICell cell, TExport item)
        {
            if (column.Type != typeof(float) && column.Type != typeof(float?))
                throw new InvalidOperationException("Invalid type of column.");
            float value;
            if (column is IExcelExportColumn<TExport, float> singleColumn)
            {
                value = singleColumn.Read(item);
            }
            else if (column is IExcelExportColumn<TExport, float?> nullableSingleColumn)
            {
                var nullableValue = nullableSingleColumn.Read(item);
                if (nullableValue.HasValue == false)
                    return;
                value = nullableValue.Value;
            }
            else
            {
                throw new ArgumentException($"Export column does not implement \"{typeof(IExcelExportColumn<TExport, float>).FullName}\" or \"{typeof(IExcelExportColumn<TExport, float?>).FullName}\".");
            }
            cell.SetCellValue(value);
        }
    }
}
