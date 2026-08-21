using NPOI.SS.UserModel;
using System;

namespace Wodsoft.ComBoost.ExcelExport.NPOI.CellWriters
{
    /// <summary>
    /// Writes <see cref="char"/> values to NPOI cells.
    /// </summary>
    public class NpoiExcelExportCellCharWriter : INpoiExcelExportCellWriter
    {
        /// <inheritdoc />
        public bool CanWrite(Type type) => type == typeof(char);

        /// <inheritdoc />
        public void Write<TExport>(IExcelExportColumn<TExport> column, ICell cell, TExport item)
        {
            if (column.Type != typeof(char) && column.Type != typeof(char?))
                throw new InvalidOperationException("Invalid type of column.");
            char value;
            if (column is IExcelExportColumn<TExport, char> charColumn)
            {
                value = charColumn.Read(item);
            }
            else if (column is IExcelExportColumn<TExport, char?> nullableCharColumn)
            {
                var nullableValue = nullableCharColumn.Read(item);
                if (nullableValue.HasValue == false)
                    return;
                value = nullableValue.Value;
            }
            else
            {
                throw new ArgumentException($"Export column does not implement \"{typeof(IExcelExportColumn<TExport, char>).FullName}\" or \"{typeof(IExcelExportColumn<TExport, char?>).FullName}\".");
            }
            cell.SetCellValue(value.ToString());
        }
    }
}
