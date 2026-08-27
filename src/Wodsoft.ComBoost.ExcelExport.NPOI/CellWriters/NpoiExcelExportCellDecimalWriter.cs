using NPOI.SS.UserModel;
using System;

namespace Wodsoft.ComBoost.ExcelExport.NPOI.CellWriters
{
    /// <summary>
    /// Writes <see cref="decimal"/> values to NPOI cells.
    /// </summary>
    public class NpoiExcelExportCellDecimalWriter : INpoiExcelExportCellWriter
    {
        /// <inheritdoc />
        public bool CanWrite(Type type) => type == typeof(decimal);

        /// <inheritdoc />
        public void Write<TExport>(NpoiExcelExportService service, NpoiExcelExportContext context, IExcelExportColumn<TExport> column, ICell cell, TExport item)
        {
            if (column.Type != typeof(decimal) && column.Type != typeof(decimal?))
                throw new InvalidOperationException("Invalid type of column.");
            decimal value;
            if (column is IExcelExportColumn<TExport, decimal> decimalColumn)
            {
                value = decimalColumn.Read(item);
            }
            else if (column is IExcelExportColumn<TExport, decimal?> nullableDecimalColumn)
            {
                var nullableValue = nullableDecimalColumn.Read(item);
                if (nullableValue.HasValue == false)
                    return;
                value = nullableValue.Value;
            }
            else
            {
                throw new ArgumentException($"Export column does not implement \"{typeof(IExcelExportColumn<TExport, decimal>).FullName}\" or \"{typeof(IExcelExportColumn<TExport, decimal?>).FullName}\".");
            }
            service.WriteCell(context, cell, column, item, (double)value);
        }
    }
}
