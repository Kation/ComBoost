using NPOI.SS.UserModel;
using System;

namespace Wodsoft.ComBoost.ExcelExport.NPOI.CellWriters
{
    /// <summary>
    /// Writes <see cref="bool"/> values to NPOI cells.
    /// </summary>
    public class NpoiExcelExportCellBooleanWriter : INpoiExcelExportCellWriter
    {
        /// <inheritdoc />
        public bool CanWrite(Type type) => type == typeof(bool);

        /// <inheritdoc />
        public void Write<TExport>(NpoiExcelExportService service, NpoiExcelExportContext context, IExcelExportColumn<TExport> column, ICell cell, TExport item)
        {
            if (column.Type != typeof(bool) && column.Type != typeof(bool?))
                throw new InvalidOperationException("Invalid type of column.");
            bool value;
            if (column is IExcelExportColumn<TExport, bool> boolColumn)
            {
                value = boolColumn.Read(item);
            }
            else if (column is IExcelExportColumn<TExport, bool?> nullableBoolColumn)
            {
                var nullableValue = nullableBoolColumn.Read(item);
                if (nullableValue.HasValue == false)
                    return;
                value = nullableValue.Value;
            }
            else
            {
                throw new ArgumentException($"Export column does not implement \"{typeof(IExcelExportColumn<TExport, bool>).FullName}\" or \"{typeof(IExcelExportColumn<TExport, bool?>).FullName}\".");
            }
            service.WriteCell(context, cell, column, item, value);
        }
    }
}
