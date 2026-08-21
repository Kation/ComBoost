using System.Collections;

namespace Wodsoft.ComBoost.ExcelExport
{
    /// <summary>
    /// Expands a nested collection on an exported item into additional columns or rows.
    /// </summary>
    /// <typeparam name="TExport">The parent exported item type.</typeparam>
    public class ExcelExportExpandingFeature<TExport> : IExcelExportExpandingFeature<TExport>
    {
        private readonly Func<TExport, IEnumerable> _getItemsFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelExportExpandingFeature{TExport}"/> class.
        /// </summary>
        /// <param name="itemType">The type of each expanded item.</param>
        /// <param name="columns">The columns used to export each expanded item.</param>
        /// <param name="getItemsFunc">A function that reads nested items from a parent instance.</param>
        public ExcelExportExpandingFeature(Type itemType, IList<IExcelExportColumn> columns, Func<TExport, IEnumerable> getItemsFunc)
        {
            ItemType = itemType;
            Columns = columns;
            _getItemsFunc = getItemsFunc;
        }

        /// <inheritdoc />
        public Type ItemType { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the parent column name is written and merged over the child headers.
        /// The default is <see langword="true"/>.
        /// </summary>
        public bool MergeHeader { get; set; } = true;

        /// <inheritdoc />
        public IList<IExcelExportColumn> Columns { get; }

        /// <inheritdoc />
        public IEnumerable GetItems(TExport instance)
        {
            return _getItemsFunc(instance);
        }
    }
}
