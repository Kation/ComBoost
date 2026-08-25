using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Wodsoft.ComBoost.ExcelExport
{
    /// <summary>
    /// Describes a non-generic export column.
    /// </summary>
    public interface IExcelExportColumn
    {
        /// <summary>
        /// Gets or sets the column header name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets the CLR property associated with this column, or <see langword="null"/> if the column is not bound to a CLR property.
        /// </summary>
        PropertyInfo? ClrProperty { get; }

        /// <summary>
        /// Gets the CLR type of values written by this column.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Gets or sets the optional Excel column width.
        /// </summary>
        int? Width { get; set; }

        /// <summary>
        /// Gets the features attached to this column.
        /// </summary>
        IList<IExcelExportColumnFeature> Features { get; }

#if NETSTANDARD2_0
        /// <summary>
        /// Tries to get the first column feature of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The feature type.</typeparam>
        /// <param name="feature">When this method returns, the matching feature, or <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if a feature was found; otherwise, <see langword="false"/>.</returns>
        bool TryGetFeature<T>(out T? feature) where T : class, IExcelExportColumnFeature;
#else
        /// <summary>
        /// Tries to get the first column feature of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The feature type.</typeparam>
        /// <param name="feature">When this method returns, the matching feature, or <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if a feature was found; otherwise, <see langword="false"/>.</returns>
        bool TryGetFeature<T>([NotNullWhen(true)] out T? feature) where T : class, IExcelExportColumnFeature;
#endif

        /// <summary>
        /// Creates a shallow copy of this column, including its features.
        /// </summary>
        /// <returns>A cloned column instance.</returns>
        IExcelExportColumn Clone();
    }

    /// <summary>
    /// Describes an export column for items of type <typeparamref name="TExport"/>.
    /// </summary>
    /// <typeparam name="TExport">The exported item type.</typeparam>
    public interface IExcelExportColumn<TExport> : IExcelExportColumn
    {
        /// <summary>
        /// Creates a shallow copy of this column, including its features.
        /// </summary>
        /// <returns>A cloned column instance.</returns>
        new IExcelExportColumn<TExport> Clone();
    }

    /// <summary>
    /// Describes a typed export column that can read values from an exported item.
    /// </summary>
    /// <typeparam name="TExport">The exported item type.</typeparam>
    /// <typeparam name="TValue">The value type written to the cell.</typeparam>
    public interface IExcelExportColumn<TExport, TValue> : IExcelExportColumn<TExport>
    {
        /// <summary>
        /// Reads the column value from the specified item.
        /// </summary>
        /// <param name="instance">The exported item.</param>
        /// <returns>The value to write.</returns>
        TValue Read(TExport instance);

        /// <summary>
        /// Creates a column that reads a different value while keeping the current name, width, and features.
        /// </summary>
        /// <typeparam name="TNewValue">The new value type.</typeparam>
        /// <param name="reader">A function that reads the value from an exported item.</param>
        /// <returns>A new column instance.</returns>
        IExcelExportColumn<TExport, TNewValue> Override<TNewValue>(Func<TExport, TNewValue> reader);

        /// <summary>
        /// Creates a shallow copy of this column, including its features.
        /// </summary>
        /// <returns>A cloned column instance.</returns>
        new IExcelExportColumn<TExport, TValue> Clone();
    }
}
