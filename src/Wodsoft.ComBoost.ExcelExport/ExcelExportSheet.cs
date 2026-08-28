using System.Diagnostics.CodeAnalysis;

namespace Wodsoft.ComBoost.ExcelExport
{
    /// <summary>
    /// A default implementation of <see cref="IExcelExportSheet{TExport}"/>.
    /// </summary>
    /// <typeparam name="TExport">The exported item type.</typeparam>
    public class ExcelExportSheet<TExport> : IExcelExportSheet<TExport>
    {
        /// <summary>
        /// Initializes an unnamed sheet with no columns.
        /// </summary>
        public ExcelExportSheet() : this(null, new List<IExcelExportColumn<TExport>>()) { }

        /// <summary>
        /// Initializes an unnamed sheet with the specified columns.
        /// </summary>
        /// <param name="columns">The sheet columns.</param>
        public ExcelExportSheet(IList<IExcelExportColumn<TExport>> columns) : this(null, columns) { }

        /// <summary>
        /// Initializes a named sheet with no columns.
        /// </summary>
        /// <param name="name">The sheet name.</param>
        public ExcelExportSheet(string name) : this(name, new List<IExcelExportColumn<TExport>>()) { }

        /// <summary>
        /// Initializes a sheet with the specified name and columns.
        /// </summary>
        /// <param name="name">The sheet name, or <see langword="null"/> to use the workbook default.</param>
        /// <param name="columns">The sheet columns.</param>
        public ExcelExportSheet(string? name, IList<IExcelExportColumn<TExport>> columns) :
            this(name, columns, new List<IExcelExportSheetFeature>())
        {
            Name = name;
            Columns = columns;
        }

        private ExcelExportSheet(string? name, IList<IExcelExportColumn<TExport>> columns, IList<IExcelExportSheetFeature> features)
        {
            Name = name;
            Columns = columns;
            Features = features;
        }

        /// <inheritdoc />
        public IList<IExcelExportColumn<TExport>> Columns { get; }

        /// <inheritdoc />
        public string? Name { get; set; }

        /// <inheritdoc />
        public IList<IExcelExportSheetFeature> Features { get; }

        /// <inheritdoc />
        public bool CreateHeaders { get; set; } = true;

        /// <inheritdoc />
        public int StartRow { get; set; }

        /// <inheritdoc />
        public int StartColumn { get; set; }

        /// <inheritdoc />
        public IExcelExportSheet<TExport> Clone()
        {
            return new ExcelExportSheet<TExport>(Name, new List<IExcelExportColumn<TExport>>(Columns), new List<IExcelExportSheetFeature>(Features))
            {
                CreateHeaders = CreateHeaders, 
                StartRow = StartRow, 
                StartColumn = StartColumn
            };
        }

        /// <inheritdoc />
#if NETSTANDARD2_0
        public bool TryGetFeature<T>(out T? feature) where T : class, IExcelExportSheetFeature
#else
        public bool TryGetFeature<T>([NotNullWhen(true)] out T? feature) where T : class, IExcelExportSheetFeature
#endif
        {
            foreach (var item in Features)
            {
                if (item is T value)
                {
                    feature = value;
                    return true;
                }
            }
            feature = null;
            return false;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Sheet : {Name}";
        }
    }
}
