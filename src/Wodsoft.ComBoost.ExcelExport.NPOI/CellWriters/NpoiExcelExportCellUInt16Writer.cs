using NPOI.SS.UserModel;
using System;

namespace Wodsoft.ComBoost.ExcelExport.NPOI.CellWriters
{
    /// <summary>
    /// Writes <see cref="ushort"/> values to NPOI cells.
    /// </summary>
    public class NpoiExcelExportCellUInt16Writer : INpoiExcelExportCellWriter
    {
        /// <inheritdoc />
        public bool CanWrite(Type type) => type == typeof(ushort);

        /// <inheritdoc />
        public void Write<TExport>(NpoiExcelExportService service, NpoiExcelExportContext context, IExcelExportColumn<TExport> column, ICell cell, TExport item)
        {
            if (column.Type != typeof(ushort) && column.Type != typeof(ushort?))
                throw new InvalidOperationException("Invalid type of column.");
            ushort value;
            if (column is IExcelExportColumn<TExport, ushort> uint16Column)
            {
                value = uint16Column.Read(item);
            }
            else if (column is IExcelExportColumn<TExport, ushort?> nullableUInt16Column)
            {
                var nullableValue = nullableUInt16Column.Read(item);
                if (nullableValue.HasValue == false)
                    return;
                value = nullableValue.Value;
            }
            else
            {
                throw new ArgumentException($"Export column does not implement \"{typeof(IExcelExportColumn<TExport, ushort>).FullName}\" or \"{typeof(IExcelExportColumn<TExport, ushort?>).FullName}\".");
            }
            service.WriteCell(context, cell, column, item, value);
        }
    }
}
