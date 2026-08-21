using NPOI.SS.UserModel;

namespace Wodsoft.ComBoost.ExcelExport.NPOI
{
    /// <summary>
    /// Writes a typed value to an NPOI cell.
    /// </summary>
    public interface INpoiExcelExportCellWriter
    {
        /// <summary>
        /// Determines whether this writer can write values of the specified type.
        /// </summary>
        /// <param name="type">The CLR type, typically after unwrapping <see cref="Nullable{T}"/>.</param>
        /// <returns><see langword="true"/> if this writer supports <paramref name="type"/>; otherwise, <see langword="false"/>.</returns>
        bool CanWrite(Type type);

        /// <summary>
        /// Writes the column value of <paramref name="item"/> to <paramref name="cell"/>.
        /// </summary>
        /// <typeparam name="TExport">The exported item type.</typeparam>
        /// <param name="column">The column that supplies the value.</param>
        /// <param name="cell">The target cell.</param>
        /// <param name="item">The exported item.</param>
        void Write<TExport>(IExcelExportColumn<TExport> column, ICell cell, TExport item);
    }
}
