using System.Collections;

namespace Wodsoft.ComBoost.ExcelExport
{
    /// <summary>
    /// Expands a nested collection on an exported item into additional columns or rows.
    /// </summary>
    /// <typeparam name="TExport">The parent exported item type.</typeparam>
    public interface IExcelExportExpandingFeature<TExport> : IExcelExportColumnFeature
    {
        /// <summary>
        /// Gets the type of each expanded item.
        /// </summary>
        Type ItemType { get; }

        /// <summary>
        /// Gets a value indicating whether the parent column name is written and merged over the child headers.
        /// When <see langword="false"/>, only child column headers are written.
        /// </summary>
        bool MergeHeader { get; }

        /// <summary>
        /// Gets the columns used to export each expanded item.
        /// </summary>
        IList<IExcelExportColumn> Columns { get; }

        /// <summary>
        /// Gets the nested items from the specified parent instance.
        /// </summary>
        /// <param name="instance">The parent exported item.</param>
        /// <returns>The items to expand.</returns>
        IEnumerable GetItems(TExport instance);
    }
}
