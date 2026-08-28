using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Wodsoft.ComBoost.Data.Entity.Metadata;

namespace Wodsoft.ComBoost.ExcelExport
{
    /// <summary>
    /// Provides shared sheet-building logic for Excel export implementations.
    /// </summary>
    public abstract class ExcelExportService : IExcelExportService
    {
        /// <inheritdoc />
        public IExcelExportContext CreateContext() => CreateContext(null);

        /// <inheritdoc />
        public abstract IExcelExportContext CreateContext(IServiceProvider? services);

        /// <inheritdoc />
        public abstract void Export<TExport>(IExcelExportContext context, IExcelExportSheet<TExport> sheet, IEnumerable<TExport> source);

        /// <inheritdoc />
        public abstract Task ExportAsync<TExport>(IExcelExportContext context, IExcelExportSheet<TExport> sheet, IAsyncEnumerable<TExport> source);

        /// <inheritdoc />
        public virtual IExcelExportSheet<TExport> GetExportSheet<TExport>()
        {
#if NET7_0_OR_GREATER
            if (typeof(TExport).IsAssignableFrom(typeof(IExcelExportSheetMetadata<TExport>)))
            {
                return (IExcelExportSheet<TExport>)typeof(ExcelExportSheetMetadataAccessor).GetMethod("GetMetadata", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)!
                    .Invoke(null, null)!;
            }
#else
            var sheetProperty = typeof(TExport).GetProperty("Sheet", BindingFlags.Public | BindingFlags.Static);
            if (sheetProperty != null && sheetProperty.PropertyType == typeof(IExcelExportSheet<TExport>))
                return (IExcelExportSheet<TExport>)sheetProperty.GetValue(null);
#endif
            return CreateSheet<TExport>();
        }

        /// <summary>
        /// Creates a sheet from entity metadata of <typeparamref name="TExport"/>.
        /// </summary>
        /// <typeparam name="TExport">The exported item type.</typeparam>
        /// <returns>The generated sheet definition.</returns>
        protected virtual IExcelExportSheet<TExport> CreateSheet<TExport>()
        {
            var metadata = EntityDescriptor.GetMetadata<TExport>();
            List<IExcelExportColumn<TExport>> columns = GetColumns<TExport>(metadata);
            return new ExcelExportSheet<TExport>(metadata.Name, columns);
        }

        /// <summary>
        /// Builds export columns from entity metadata.
        /// </summary>
        /// <typeparam name="TExport">The exported item type.</typeparam>
        /// <param name="metadata">The entity metadata.</param>
        /// <returns>The generated columns.</returns>
        protected virtual List<IExcelExportColumn<TExport>> GetColumns<TExport>(IEntityMetadata metadata)
        {
            List<IExcelExportColumn<TExport>> columns = new List<IExcelExportColumn<TExport>>();
            foreach (var item in metadata.Properties
                .Where(t => t.GetAttribute<HideAttribute>() == null || t.GetAttribute<HideAttribute>()!.IsHiddenOnView == false)
                .OrderBy(t => t.Order))
            {
                columns.Add(GetColumn<TExport>(item));
            }
            return columns;
        }

        private readonly ConcurrentDictionary<Type, Func<ExcelExportService, IPropertyMetadata, IExcelExportColumn>> _getColumnCache = new ConcurrentDictionary<Type, Func<ExcelExportService, IPropertyMetadata, IExcelExportColumn>>();
        private static readonly MethodInfo _GetColumnMethod = typeof(ExcelExportService).GetMethod("GetColumn", BindingFlags.Instance | BindingFlags.NonPublic)!;
        private List<IExcelExportColumn> GetColumns(Type type)
        {
            var getColumnFunc = _getColumnCache.GetOrAdd(type, t =>
            {
                var serviceParameter = Expression.Parameter(typeof(ExcelExportService));
                var metadataParameter = Expression.Parameter(typeof(IPropertyMetadata));
                return Expression.Lambda<Func<ExcelExportService, IPropertyMetadata, IExcelExportColumn>>(
                    Expression.Call(serviceParameter, _GetColumnMethod.MakeGenericMethod(t), metadataParameter),
                    serviceParameter,
                    metadataParameter).Compile();
            });
            var metadata = EntityDescriptor.GetMetadata(type);
            List<IExcelExportColumn> columns = new List<IExcelExportColumn>();
            foreach (var item in metadata.Properties
                .Where(t => t.GetAttribute<HideAttribute>() == null || t.GetAttribute<HideAttribute>()!.IsHiddenOnView == false)
                .OrderBy(t => t.Order))
            {
                columns.Add(getColumnFunc(this, item));
            }
            return columns;
        }

        /// <summary>
        /// Builds an export column for the specified property of <typeparamref name="TExport"/>.
        /// </summary>
        /// <typeparam name="TExport">The exported item type.</typeparam>
        /// <param name="propertyName">The CLR property name.</param>
        /// <returns>The generated column, including features from export attributes.</returns>
        /// <exception cref="ArgumentException">No property named <paramref name="propertyName"/> exists on <typeparamref name="TExport"/>.</exception>
        public virtual IExcelExportColumn<TExport> GetExportColumn<TExport>(string propertyName)
        {
            var metadata = EntityDescriptor.GetMetadata<TExport>();
            var property = metadata.GetProperty(propertyName) ?? throw new ArgumentException("Property not found.");
            return GetColumn<TExport>(property);
        }

        /// <summary>
        /// Builds an export column for the property selected by <paramref name="propertySelector"/>.
        /// </summary>
        /// <typeparam name="TExport">The exported item type.</typeparam>
        /// <typeparam name="TProperty">The selected property type.</typeparam>
        /// <param name="propertySelector">A property access expression on <typeparamref name="TExport"/>.</param>
        /// <returns>The generated column, including features from export attributes.</returns>
        /// <exception cref="InvalidOperationException">The expression is not a property access, or the property is not found in metadata.</exception>
        public virtual IExcelExportColumn<TExport> GetExportColumn<TExport, TProperty>(Expression<Func<TExport, TProperty>> propertySelector)
        {
            var metadata = EntityDescriptor.GetMetadata<TExport>(); MemberExpression propertyExpression = propertySelector.Body as MemberExpression ?? throw new InvalidOperationException("Lambda expression must specifically a property.");
            var property = metadata.GetProperty(propertyExpression.Member.Name) ?? throw new InvalidOperationException($"Sheet doesn't exist any column with CLR property of \"{propertyExpression.Member}\".");
            return GetColumn<TExport>(property);
        }

        /// <summary>
        /// Creates a column for the specified property and applies export attributes.
        /// </summary>
        /// <typeparam name="TExport">The exported item type.</typeparam>
        /// <param name="property">The property metadata.</param>
        /// <returns>The generated column.</returns>
        protected virtual IExcelExportColumn<TExport> GetColumn<TExport>(IPropertyMetadata property)
        {
            var column = ExcelExportSheetMetadataAccessor.CreateColumn<TExport>(property);
            var colorAttr = property.GetAttribute<ExcelExportColorAttribute>();
            if (colorAttr != null)
            {
                column.Features.Add(new ExcelExportColorFeature(
                    colorAttr.GetHeaderBackGround(),
                    colorAttr.GetHeaderFrontGround(),
                    colorAttr.GetContentBackGround(),
                    colorAttr.GetContentFrontGround()));
            }
            var displayFormatAttr = property.GetAttribute<DisplayFormatAttribute>();
            if (displayFormatAttr != null && displayFormatAttr.DataFormatString is not null)
                column.Features.Add(new ExcelExportDataFormatFeature(ConvertToExcelFormat(displayFormatAttr.DataFormatString)));
            else
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTimeOffset) || property.ClrType == typeof(DateTime?) || property.ClrType == typeof(DateTimeOffset?))
                    column.Features.Add(new ExcelExportDataFormatFeature(ConvertToExcelFormat(CultureInfo.CurrentUICulture.DateTimeFormat.FullDateTimePattern)));
#if NET6_0_OR_GREATER
                else if (property.ClrType == typeof(DateOnly) || property.ClrType == typeof(DateOnly?))
                    column.Features.Add(new ExcelExportDataFormatFeature(ConvertToExcelFormat(CultureInfo.CurrentUICulture.DateTimeFormat.LongDatePattern)));
                else if (property.ClrType == typeof(TimeOnly) || property.ClrType == typeof(TimeOnly?))
                    column.Features.Add(new ExcelExportDataFormatFeature(ConvertToExcelFormat(CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern)));
#endif
            }
            var expandAttribute = property.GetAttribute<ExcelExportExpandableAttribute>();
            if (expandAttribute != null)
            {
                Type? itemType = expandAttribute.ItemType;
                if (itemType == null)
                {
                    var enumerableInterface = column.Type.GetInterfaces().FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                    if (enumerableInterface == null)
                        throw new InvalidOperationException($"Property \"{property.Name}\" defined expandable without item type but property type doesn't implement any \"IEnumerable<>\" interface.");
                    itemType = enumerableInterface.GetGenericArguments()[0];
                }
                else
                {
                    var enumerableInterface = typeof(IEnumerable<>).MakeGenericType(itemType);
                    if (!column.Type.GetInterfaces().Any(t => t == enumerableInterface))
                        throw new InvalidOperationException($"Property \"{property.Name}\" defined expandable with item type but property type doesn't implement the \"IEnumerable<ItemType>\" interface.");
                }
                Func<TExport, IEnumerable> getItemsFunc;
                var parameter = Expression.Parameter(typeof(TExport));
                if (property.TryGetPropertyInfo(out var propertyInfo))
                {
                    getItemsFunc = Expression.Lambda<Func<TExport, IEnumerable>>(Expression.Property(parameter, propertyInfo), parameter).Compile();
                }
                else
                {
                    var readDelegate = Expression.Constant((Func<object, object?>)property.GetValue);
                    getItemsFunc = Expression.Lambda<Func<TExport, IEnumerable>>(
                        Expression.Convert(Expression.Invoke(readDelegate, Expression.Convert(parameter, typeof(object))), typeof(IEnumerable)),
                        parameter).Compile();
                }
                column.Features.Add(new ExcelExportExpandingFeature<TExport>(itemType, GetColumns(itemType), getItemsFunc));
            }
            var enumType = Nullable.GetUnderlyingType(column.Type) ?? column.Type;
            if (enumType.IsEnum)
            {
                column.Features.Add(new ExcelExportValidationFeature(ExcelExportEnumHelper.GetDisplayNames(enumType)));
            }
            var widthAttr = property.GetAttribute<ExcelExportWidthAttribute>();
            if (widthAttr != null)
            {
                column.Width = widthAttr.Width;
            }
            return column;
        }

        /// <summary>
        /// Converts a .NET format string, typically from <see cref="DisplayFormatAttribute"/>, to an Excel data format.
        /// </summary>
        /// <param name="format">The .NET format string.</param>
        /// <returns>An Excel data format string.</returns>
        protected virtual string ConvertToExcelFormat(string format)
        {
#if NET7_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(format);
#else
            if (format == null)
                throw new ArgumentNullException(nameof(format));
#endif
            if (format.Length == 0)
                return format;

            format = UnwrapDataFormatString(format);
            if (format.Length == 0)
                return "General";

            if (TryMapStandardNumericFormat(format, out var numericFormat))
                return numericFormat;

            if (TryMapStandardDateTimeFormat(format, out var dateFormat))
                return dateFormat;

            return ConvertCustomFormat(format);
        }

        private static string UnwrapDataFormatString(string format)
        {
            if (format.Length < 3 || format[0] != '{')
                return format;

            var close = format.LastIndexOf('}');
            if (close != format.Length - 1)
                return format;

            var inner = format.AsSpan(1, close - 1).Trim();
            if (inner.Length == 0 || inner[0] != '0')
                return format;

            inner = inner.Slice(1).TrimStart();
            if (inner.Length == 0)
                return string.Empty;

            if (inner[0] == ',')
            {
                inner = inner.Slice(1);
                int i = 0;
                if (i < inner.Length && inner[i] == '-')
                    i++;
                while (i < inner.Length && char.IsDigit(inner[i]))
                    i++;
                inner = inner.Slice(1).TrimStart();
                if (inner.Length == 0)
                    return string.Empty;
            }

            if (inner[0] != ':')
                return format;

            return inner.Slice(1).ToString();
        }

#if NETSTANDARD2_0
        private static bool TryMapStandardNumericFormat(string format, out string? excelFormat)
#else
        private static bool TryMapStandardNumericFormat(string format, [NotNullWhen(true)] out string? excelFormat)
#endif
        {
            excelFormat = null;
            if (format.Length is 0 or > 3)
                return false;

            char specifier = format[0];
            int? precision = null;
            if (format.Length > 1)
            {
#if NET8_0_OR_GREATER
                if (!int.TryParse(format.AsSpan(1), out var parsed) || parsed < 0)
#else
                if (!int.TryParse(format.Substring(1), out var parsed) || parsed < 0)
#endif
                    return false;
                precision = parsed;
            }

            if (precision is null && IsStandardDateTimeSpecifier(format))
                return false;

            switch (specifier)
            {
                case 'C' or 'c':
                    excelFormat = BuildCurrencyFormat(precision ?? CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits);
                    return true;
                case 'D' or 'd':
                    excelFormat = precision is null or 0 ? "0" : new string('0', precision.Value);
                    return true;
                case 'E' or 'e':
                    var exponentialDecimals = precision ?? 6;
                    excelFormat = "0." + new string('0', exponentialDecimals) + (char.IsUpper(specifier) ? "E+00" : "e+00");
                    return true;
                case 'F' or 'f':
                    var fixedDecimals = precision ?? 2;
                    excelFormat = fixedDecimals == 0 ? "0" : "0." + new string('0', fixedDecimals);
                    return true;
                case 'N' or 'n':
                    var numberDecimals = precision ?? 2;
                    excelFormat = numberDecimals == 0 ? "#,##0" : "#,##0." + new string('0', numberDecimals);
                    return true;
                case 'P' or 'p':
                    var percentDecimals = precision ?? 2;
                    excelFormat = percentDecimals == 0 ? "0%" : "0." + new string('0', percentDecimals) + "%";
                    return true;
                case 'G' or 'g':
                case 'R' or 'r':
                case 'X' or 'x':
                    excelFormat = "General";
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsStandardDateTimeSpecifier(string format)
        {
            return format.Length == 1 && format[0] switch
            {
                'd' or 'D' or 'f' or 'F' or 'g' or 'G' or 'm' or 'M' or 'o' or 'O'
                    or 'r' or 'R' or 's' or 't' or 'T' or 'u' or 'U' or 'y' or 'Y' => true,
                _ => false
            };
        }

        private static string BuildCurrencyFormat(int decimals)
        {
            var symbol = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol.Replace("\"", "\"\"");
            var number = decimals <= 0 ? "#,##0" : "#,##0." + new string('0', decimals);
            return $"\"{symbol}\"{number}";
        }

#if NETSTANDARD2_0
        private static bool TryMapStandardDateTimeFormat(string format, out string? excelFormat)
#else
        private static bool TryMapStandardDateTimeFormat(string format, [NotNullWhen(true)] out string? excelFormat)
#endif
        {
            excelFormat = null;
            if (!IsStandardDateTimeSpecifier(format))
                return false;

            var dtf = CultureInfo.CurrentCulture.DateTimeFormat;
            var pattern = format[0] switch
            {
                'd' => dtf.ShortDatePattern,
                'D' => dtf.LongDatePattern,
                't' => dtf.ShortTimePattern,
                'T' => dtf.LongTimePattern,
                'f' => dtf.LongDatePattern + " " + dtf.ShortTimePattern,
                'F' => dtf.FullDateTimePattern,
                'g' => dtf.ShortDatePattern + " " + dtf.ShortTimePattern,
                'G' => dtf.ShortDatePattern + " " + dtf.LongTimePattern,
                'm' or 'M' => dtf.MonthDayPattern,
                'y' or 'Y' => dtf.YearMonthPattern,
                'o' or 'O' => "yyyy-MM-dd'T'HH:mm:ss.fffffffK",
                's' => "yyyy-MM-dd'T'HH:mm:ss",
                'u' => "yyyy-MM-dd HH:mm:ss'Z'",
                'r' or 'R' => "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
                'U' => dtf.FullDateTimePattern,
                _ => null
            };
            if (pattern is null)
                return false;

            excelFormat = ConvertCustomFormat(pattern);
            return true;
        }

        private static string ConvertCustomFormat(string format)
        {
            var builder = new StringBuilder(format.Length);
            for (int i = 0; i < format.Length;)
            {
                char c = format[i];
                if (c == '\'')
                {
                    i++;
                    var literal = new StringBuilder();
                    while (i < format.Length)
                    {
                        if (format[i] == '\'')
                        {
                            if (i + 1 < format.Length && format[i + 1] == '\'')
                            {
                                literal.Append('\'');
                                i += 2;
                                continue;
                            }
                            i++;
                            break;
                        }
                        literal.Append(format[i++]);
                    }
                    AppendExcelLiteral(builder, literal.ToString());
                    continue;
                }

                if (c == '\\' && i + 1 < format.Length)
                {
                    AppendExcelLiteral(builder, format[i + 1].ToString());
                    i += 2;
                    continue;
                }

                if (c == '%' && i + 1 < format.Length && TryConsumeDateToken(format, i + 1, out var percentLength, out var percentToken))
                {
                    builder.Append(percentToken);
                    i += 1 + percentLength;
                    continue;
                }

                if (TryConsumeDateToken(format, i, out var length, out var token))
                {
                    builder.Append(token);
                    i += length;
                    continue;
                }

                builder.Append(c);
                i++;
            }
            return builder.ToString();
        }

        private static bool TryConsumeDateToken(string format, int index, out int length, out string excelToken)
        {
            length = 0;
            excelToken = string.Empty;
            char c = format[index];
            int count = CountRepeats(format, index, c);

            switch (c)
            {
                case 'y':
                    length = count;
                    excelToken = count >= 3 ? "yyyy" : count == 2 ? "yy" : "y";
                    return true;
                case 'M':
                    length = count;
                    excelToken = count >= 4 ? "mmmm" : count == 3 ? "mmm" : count == 2 ? "mm" : "m";
                    return true;
                case 'd':
                    length = count;
                    excelToken = count >= 4 ? "dddd" : count == 3 ? "ddd" : count == 2 ? "dd" : "d";
                    return true;
                case 'H' or 'h':
                    length = count;
                    excelToken = count >= 2 ? "hh" : "h";
                    return true;
                case 'm':
                    length = count;
                    excelToken = count >= 2 ? "mm" : "m";
                    return true;
                case 's':
                    length = count;
                    excelToken = count >= 2 ? "ss" : "s";
                    return true;
                case 'f' or 'F':
                    length = count;
                    excelToken = new string('0', Math.Min(count, 3));
                    return true;
                case 't':
                    length = count;
                    excelToken = count >= 2 ? "AM/PM" : "A/P";
                    return true;
                case 'K':
                    length = 1;
                    excelToken = string.Empty;
                    return true;
                case 'z':
                    length = count;
                    excelToken = string.Empty;
                    return true;
                default:
                    return false;
            }
        }

        private static int CountRepeats(string format, int index, char value)
        {
            int count = 0;
            while (index + count < format.Length && format[index + count] == value)
                count++;
            return count;
        }

        private static void AppendExcelLiteral(StringBuilder builder, string literal)
        {
            if (literal.Length == 0)
                return;
            builder.Append('"').Append(literal.Replace("\"", "\"\"")).Append('"');
        }
    }

    internal static class ExcelExportSheetMetadataAccessor
    {
#if NET7_0_OR_GREATER
        public static IExcelExportSheet<TExport> GetMetadata<TExport>() where TExport : IExcelExportSheetMetadata<TExport> => TExport.Sheet;
#endif

        public static IExcelExportColumn<TExport, TValue> CreateColumn<TExport, TValue>(IPropertyMetadata property)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TExport));
            Func<TExport, TValue> reader;
            if (property.TryGetPropertyInfo(out var propertyInfo))
            {
                if (propertyInfo.PropertyType != typeof(TValue))
                    throw new InvalidOperationException($"Property type is not \"{typeof(TValue).FullName}\"");
                reader = Expression.Lambda<Func<TExport, TValue>>(Expression.Property(parameter, propertyInfo), parameter).Compile();
            }
            else
            {
                var readDelegate = Expression.Constant((Func<object, object?>)property.GetValue);
                reader = Expression.Lambda<Func<TExport, TValue>>(
                    Expression.Convert(Expression.Invoke(readDelegate, Expression.Convert(parameter, typeof(object))), typeof(TValue)),
                    parameter).Compile();
            }
            return new ExcelExportColumn<TExport, TValue>(property.Name, propertyInfo, reader);
        }

        private static readonly ConcurrentDictionary<IPropertyMetadata, Delegate> _CreateColumnCache = new ConcurrentDictionary<IPropertyMetadata, Delegate>();
        public static IExcelExportColumn<TExport> CreateColumn<TExport>(IPropertyMetadata property)
        {
            var func = (Func<IPropertyMetadata, IExcelExportColumn<TExport>>)_CreateColumnCache.GetOrAdd(property, p =>
            {
#if NETSTANDARD2_0
                var createMethod = typeof(ExcelExportSheetMetadataAccessor).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).First(t => t.Name == "CreateColumn" && t.IsGenericMethodDefinition && t.GetGenericArguments().Length == 2)!.MakeGenericMethod(typeof(TExport), p.ClrType);
#else
                var createMethod = typeof(ExcelExportSheetMetadataAccessor).GetMethod("CreateColumn", 2, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, null, [typeof(IPropertyMetadata)], null)!.MakeGenericMethod(typeof(TExport), p.ClrType);
#endif
                var parameter = Expression.Parameter(typeof(IPropertyMetadata));
                return Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(IPropertyMetadata), typeof(IExcelExportColumn<TExport>)),
                    Expression.Convert(Expression.Call(createMethod, parameter), typeof(IExcelExportColumn<TExport>)), parameter).Compile();
            });
            return func(property);
        }
    }
}
