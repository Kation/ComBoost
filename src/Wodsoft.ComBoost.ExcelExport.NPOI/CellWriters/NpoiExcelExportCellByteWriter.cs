using NPOI.SS.UserModel;
using System;

namespace Wodsoft.ComBoost.ExcelExport.NPOI.CellWriters
{
    /// <summary>
    /// Writes <see cref="byte"/> values to NPOI cells.
    /// </summary>
    public class NpoiExcelExportCellByteWriter : INpoiExcelExportCellWriter
    {
        /// <inheritdoc />
        public bool CanWrite(Type type) => type == typeof(byte);

        /// <inheritdoc />
        public void Write<TExport>(NpoiExcelExportService service, NpoiExcelExportContext context, IExcelExportColumn<TExport> column, ICell cell, TExport item)
        {
            if (column.Type != typeof(byte) && column.Type != typeof(byte?))
                throw new InvalidOperationException("Invalid type of column.");
            byte value;
            if (column is IExcelExportColumn<TExport, byte> byteColumn)
            {
                value = byteColumn.Read(item);
            }
            else if (column is IExcelExportColumn<TExport, byte?> nullableByteColumn)
            {
                var nullableValue = nullableByteColumn.Read(item);
                if (nullableValue.HasValue == false)
                    return;
                value = nullableValue.Value;
            }
            else
            {
                throw new ArgumentException($"Export column does not implement \"{typeof(IExcelExportColumn<TExport, byte>).FullName}\" or \"{typeof(IExcelExportColumn<TExport, byte?>).FullName}\".");
            }
            service.WriteCell(context, cell, column, item, value);
        }
    }
}
