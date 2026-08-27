using NPOI.SS.UserModel;
using System;

namespace Wodsoft.ComBoost.ExcelExport.NPOI.CellWriters
{
    /// <summary>
    /// Writes <see cref="sbyte"/> values to NPOI cells.
    /// </summary>
    public class NpoiExcelExportCellSByteWriter : INpoiExcelExportCellWriter
    {
        /// <inheritdoc />
        public bool CanWrite(Type type) => type == typeof(sbyte);

        /// <inheritdoc />
        public void Write<TExport>(NpoiExcelExportService service, NpoiExcelExportContext context, IExcelExportColumn<TExport> column, ICell cell, TExport item)
        {
            if (column.Type != typeof(sbyte) && column.Type != typeof(sbyte?))
                throw new InvalidOperationException("Invalid type of column.");
            sbyte value;
            if (column is IExcelExportColumn<TExport, sbyte> sbyteColumn)
            {
                value = sbyteColumn.Read(item);
            }
            else if (column is IExcelExportColumn<TExport, sbyte?> nullableSbyteColumn)
            {
                var nullableValue = nullableSbyteColumn.Read(item);
                if (nullableValue.HasValue == false)
                    return;
                value = nullableValue.Value;
            }
            else
            {
                throw new ArgumentException($"Export column does not implement \"{typeof(IExcelExportColumn<TExport, sbyte>).FullName}\" or \"{typeof(IExcelExportColumn<TExport, sbyte?>).FullName}\".");
            }
            service.WriteCell(context, cell, column, item, value);
        }
    }
}
