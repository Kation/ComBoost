using System.Diagnostics.CodeAnalysis;

namespace Wodsoft.ComBoost.ExcelExport
{
    /// <summary>
    /// Describes an Excel sheet used to export items of type <typeparamref name="TExport"/>.
    /// </summary>
    /// <typeparam name="TExport">The exported item type.</typeparam>
    public interface IExcelExportSheet<TExport>
    {
        /// <summary>
        /// Gets the columns written to the sheet.
        /// </summary>
        IList<IExcelExportColumn<TExport>> Columns { get; }

        /// <summary>
        /// Gets or sets the sheet name, or <see langword="null"/> to use the workbook default name.
        /// </summary>
        string? Name { get; set; }

        /// <summary>
        /// Gets the features attached to this sheet.
        /// </summary>
        IList<IExcelExportSheetFeature> Features { get; }

#if NETSTANDARD2_0
        /// <summary>
        /// Tries to get the first sheet feature of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The feature type.</typeparam>
        /// <param name="feature">When this method returns, the matching feature, or <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if a feature was found; otherwise, <see langword="false"/>.</returns>
        bool TryGetFeature<T>(out T? feature) where T : class, IExcelExportSheetFeature;
#else
        /// <summary>
        /// Tries to get the first sheet feature of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The feature type.</typeparam>
        /// <param name="feature">When this method returns, the matching feature, or <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if a feature was found; otherwise, <see langword="false"/>.</returns>
        bool TryGetFeature<T>([NotNullWhen(true)] out T? feature) where T : class, IExcelExportSheetFeature;
#endif

        /// <summary>
        /// Creates a shallow copy of this sheet, including its columns and features.
        /// </summary>
        /// <returns>A cloned sheet instance.</returns>
        IExcelExportSheet<TExport> Clone();

        /// <summary>
        /// Gets or sets a value indicating whether header cells are written before data rows.
        /// </summary>
        bool CreateHeaders { get; set; }

        /// <summary>
        /// Gets or sets the zero-based row index where the sheet starts writing.
        /// </summary>
        int StartRow { get; set; }

        /// <summary>
        /// Gets or sets the zero-based column index where the sheet starts writing.
        /// </summary>
        int StartColumn { get; set; }
    }
}
