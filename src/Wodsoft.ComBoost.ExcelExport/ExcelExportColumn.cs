using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace Wodsoft.ComBoost.ExcelExport
{
    /// <summary>
    /// Provides a base implementation of an export column for items of type <typeparamref name="TExport"/>.
    /// </summary>
    /// <typeparam name="TExport">The exported item type.</typeparam>
    public abstract class ExcelExportColumn<TExport> : IExcelExportColumn<TExport>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelExportColumn{TExport}"/> class.
        /// </summary>
        /// <param name="name">The column header name.</param>
        public ExcelExportColumn(string name) : this(name, null, new List<IExcelExportColumnFeature>()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelExportColumn{TExport}"/> class.
        /// </summary>
        /// <param name="name">The column header name.</param>
        /// <param name="clrProperty">The column CLR property.</param>
        public ExcelExportColumn(string name, PropertyInfo? clrProperty) : this(name, clrProperty, new List<IExcelExportColumnFeature>()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelExportColumn{TExport}"/> class.
        /// </summary>
        /// <param name="name">The column header name.</param>
        /// <param name="clrProperty">The column CLR property.</param>
        /// <param name="features">The column features.</param>
        public ExcelExportColumn(string name, PropertyInfo? clrProperty, IList<IExcelExportColumnFeature> features)
        {
            Name = name;
            ClrProperty = clrProperty;
            Features = features;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelExportColumn{TExport}"/> class from another column.
        /// </summary>
        /// <param name="baseColumn">The column whose name, CLR name, and width are copied.</param>
        /// <param name="features">The column features.</param>
        public ExcelExportColumn(IExcelExportColumn<TExport> baseColumn, IList<IExcelExportColumnFeature> features) : this(baseColumn.Name, baseColumn.ClrProperty, features)
        {
            Width = baseColumn.Width;
        }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public PropertyInfo? ClrProperty { get; }

        /// <inheritdoc />
        public abstract Type Type { get; }

        /// <inheritdoc />
        public int? Width { get; set; }

        /// <inheritdoc />
        public IList<IExcelExportColumnFeature> Features { get; }

        /// <inheritdoc />
        public abstract IExcelExportColumn<TExport> Override<TValue>(Func<TExport, TValue> reader);

        /// <inheritdoc />
#if NETSTANDARD2_0
        public bool TryGetFeature<T>(out T? feature) where T : class, IExcelExportColumnFeature
#else
        public bool TryGetFeature<T>([NotNullWhen(true)] out T? feature) where T : class, IExcelExportColumnFeature
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
            return $"Column : {Name}";
        }

        /// <inheritdoc />
        public abstract IExcelExportColumn<TExport> Clone();

        IExcelExportColumn IExcelExportColumn.Clone()
        {
            return Clone();
        }
    }

    /// <summary>
    /// An export column that reads a typed value from an exported item.
    /// </summary>
    /// <typeparam name="TExport">The exported item type.</typeparam>
    /// <typeparam name="TValue">The value type written to the cell.</typeparam>
    public class ExcelExportColumn<TExport, TValue> : ExcelExportColumn<TExport>, IExcelExportColumn<TExport, TValue>
    {
        private readonly Func<TExport, TValue> _reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelExportColumn{TExport, TValue}"/> class.
        /// </summary>
        /// <param name="name">The column header name.</param>
        /// <param name="clrProperty">The CLR property associated with this column, or <see langword="null"/>.</param>
        /// <param name="reader">A function that reads the value from an exported item.</param>
        public ExcelExportColumn(string name, PropertyInfo? clrProperty, Func<TExport, TValue> reader) : base(name, clrProperty)
        {
            _reader = reader;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelExportColumn{TExport, TValue}"/> class from another column.
        /// </summary>
        /// <param name="baseColumn">The column whose name, CLR name, width, and features are copied.</param>
        /// <param name="reader">A function that reads the value from an exported item.</param>
        /// <param name="features">The column features.</param>
        public ExcelExportColumn(IExcelExportColumn<TExport> baseColumn, Func<TExport, TValue> reader, IList<IExcelExportColumnFeature> features) : base(baseColumn, features)
        {
            _reader = reader;
        }

        /// <inheritdoc />
        public override Type Type => typeof(TValue);

        /// <inheritdoc />
        public override IExcelExportColumn<TExport> Override<TNewValue>(Func<TExport, TNewValue> reader)
        {
            return new ExcelExportColumn<TExport, TNewValue>(this, reader, new List<IExcelExportColumnFeature>(Features));
        }

        /// <inheritdoc />
        public TValue Read(TExport obj)
        {
            return _reader(obj);
        }

        /// <inheritdoc />
        public override IExcelExportColumn<TExport> Clone()
        {
            return new ExcelExportColumn<TExport, TValue>(this, _reader, new List<IExcelExportColumnFeature>(Features));
        }

        IExcelExportColumn<TExport, TValue> IExcelExportColumn<TExport, TValue>.Clone()
        {
            return new ExcelExportColumn<TExport, TValue>(this, _reader, new List<IExcelExportColumnFeature>(Features));
        }
    }
}
