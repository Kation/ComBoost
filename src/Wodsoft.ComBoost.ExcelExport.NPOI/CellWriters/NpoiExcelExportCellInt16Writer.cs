using NPOI.SS.UserModel;
using System;

namespace Wodsoft.ComBoost.ExcelExport.NPOI.CellWriters
{
    /// <summary>
    /// Writes <see cref="short"/> values to NPOI cells.
    /// </summary>
    public class NpoiExcelExportCellInt16Writer : INpoiExcelExportCellWriter
    {
        /// <inheritdoc />
        public bool CanWrite(Type type) => type == typeof(short);

        /// <inheritdoc />
        public void Write<TExport>(NpoiExcelExportService service, NpoiExcelExportContext context, IExcelExportColumn<TExport> column, ICell cell, TExport item)
        {
            if (column.Type != typeof(short) && column.Type != typeof(short?))
                throw new InvalidOperationException("Invalid type of column.");
            short value;
            if (column is IExcelExportColumn<TExport, short> int16Column)
            {
                value = int16Column.Read(item);
            }
            else if (column is IExcelExportColumn<TExport, short?> nullableInt16Column)
            {
                var nullableValue = nullableInt16Column.Read(item);
                if (nullableValue.HasValue == false)
                    return;
                value = nullableValue.Value;
            }
            else
            {
                throw new ArgumentException($"Export column does not implement \"{typeof(IExcelExportColumn<TExport, short>).FullName}\" or \"{typeof(IExcelExportColumn<TExport, short?>).FullName}\".");
            }
            service.WriteCell(context, cell, column, item, value);
        }
    }
}
