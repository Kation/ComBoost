using NPOI.SS.UserModel;
using System;

namespace Wodsoft.ComBoost.ExcelExport.NPOI.CellWriters
{
    /// <summary>
    /// Writes <see cref="uint"/> values to NPOI cells.
    /// </summary>
    public class NpoiExcelExportCellUInt32Writer : INpoiExcelExportCellWriter
    {
        /// <inheritdoc />
        public bool CanWrite(Type type) => type == typeof(uint);

        /// <inheritdoc />
        public void Write<TExport>(NpoiExcelExportService service, NpoiExcelExportContext context, IExcelExportColumn<TExport> column, ICell cell, TExport item)
        {
            if (column.Type != typeof(uint) && column.Type != typeof(uint?))
                throw new InvalidOperationException("Invalid type of column.");
            uint value;
            if (column is IExcelExportColumn<TExport, uint> uint32Column)
            {
                value = uint32Column.Read(item);
            }
            else if (column is IExcelExportColumn<TExport, uint?> nullableUInt32Column)
            {
                var nullableValue = nullableUInt32Column.Read(item);
                if (nullableValue.HasValue == false)
                    return;
                value = nullableValue.Value;
            }
            else
            {
                throw new ArgumentException($"Export column does not implement \"{typeof(IExcelExportColumn<TExport, uint>).FullName}\" or \"{typeof(IExcelExportColumn<TExport, uint?>).FullName}\".");
            }
            service.WriteCell(context, cell, column, item, value);
        }
    }
}
