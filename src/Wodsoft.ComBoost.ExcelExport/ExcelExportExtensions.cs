using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Wodsoft.ComBoost.ExcelExport
{
    /// <summary>
    /// Provides helpers for locating and replacing columns on an export sheet.
    /// </summary>
    public static class ExcelExportExtensions
    {
        /// <summary>
        /// Gets the column whose <see cref="IExcelExportColumn.ClrProperty"/> matches the selected property.
        /// </summary>
        /// <typeparam name="TExport">The exported item type.</typeparam>
        /// <typeparam name="TProperty">The selected property type.</typeparam>
        /// <param name="sheet">The export sheet.</param>
        /// <param name="propertySelector">A property access expression on <typeparamref name="TExport"/>.</param>
        /// <returns>The matching column.</returns>
        /// <exception cref="InvalidOperationException">The expression is not a property access, or no matching column exists.</exception>
        public static IExcelExportColumn<TExport> GetClrColumn<TExport, TProperty>(this IExcelExportSheet<TExport> sheet, Expression<Func<TExport, TProperty>> propertySelector)
        {
            MemberExpression propertyExpression = propertySelector.Body as MemberExpression ?? throw new InvalidOperationException("Lambda expression must specifically a property.");
            var column = sheet.Columns.FirstOrDefault(t => t.ClrProperty?.GetGetMethod()?.GetBaseDefinition() == ((PropertyInfo)propertyExpression.Member).GetGetMethod()?.GetBaseDefinition()) ?? throw new InvalidOperationException($"Sheet doesn't exist any column with CLR property of \"{propertyExpression.Member}\".");
            return column;
        }

        /// <summary>
        /// Gets the column whose name of <see cref="IExcelExportColumn.ClrProperty"/> matches <paramref name="clrName"/>.
        /// </summary>
        /// <typeparam name="TExport">The exported item type.</typeparam>
        /// <param name="sheet">The export sheet.</param>
        /// <param name="clrName">The CLR member name of the column.</param>
        /// <returns>The matching column.</returns>
        /// <exception cref="InvalidOperationException">No matching column exists.</exception>
        public static IExcelExportColumn<TExport> GetClrColumn<TExport>(this IExcelExportSheet<TExport> sheet, string clrName)
        {
            var column = sheet.Columns.FirstOrDefault(t => t.ClrProperty?.Name == clrName) ?? throw new InvalidOperationException($"Sheet doesn't exist any column with CLR property name of \"{clrName}\".");
            return column;
        }

        /// <summary>
        /// Replaces <paramref name="oldColumn"/> on the sheet with <paramref name="newColumn"/> at the same index.
        /// </summary>
        /// <typeparam name="TExport">The exported item type.</typeparam>
        /// <param name="sheet">The export sheet.</param>
        /// <param name="oldColumn">The column currently on the sheet.</param>
        /// <param name="newColumn">The column that replaces <paramref name="oldColumn"/>.</param>
        /// <returns>The same sheet instance.</returns>
        /// <exception cref="InvalidOperationException"><paramref name="oldColumn"/> is not on the sheet.</exception>
        public static IExcelExportSheet<TExport> ReplaceColumn<TExport>(this IExcelExportSheet<TExport> sheet, IExcelExportColumn<TExport> oldColumn, IExcelExportColumn<TExport> newColumn)
        {
            var index = sheet.Columns.IndexOf(oldColumn);
            if (index == -1)
                throw new InvalidOperationException("Old column doesn't belong to this sheet.");
            sheet.Columns[index] = newColumn;
            return sheet;
        }

        /// <summary>
        /// Replaces the column for the selected property with a column that reads values using <paramref name="valueReader"/>.
        /// </summary>
        /// <typeparam name="TExport">The exported item type.</typeparam>
        /// <typeparam name="TProperty">The selected property type.</typeparam>
        /// <typeparam name="TValue">The value type written to the cell.</typeparam>
        /// <param name="sheet">The export sheet.</param>
        /// <param name="propertySelector">A property access expression used to locate the existing column.</param>
        /// <param name="valueReader">A function that reads the new cell value from an exported item.</param>
        /// <returns>The same sheet instance.</returns>
        /// <exception cref="InvalidOperationException">The expression is not a property access, or no matching column exists.</exception>
        public static IExcelExportSheet<TExport> OverrideColumn<TExport, TProperty, TValue>(this IExcelExportSheet<TExport> sheet, Expression<Func<TExport, TProperty>> propertySelector, Func<TExport, TValue> valueReader)
        {
            var column = GetClrColumn(sheet, propertySelector);
            var newColumn = column.Override(valueReader);
            ReplaceColumn(sheet, column, newColumn);
            return sheet;
        }

        /// <summary>
        /// Replaces the column for the selected property with a column that converts the property value using <paramref name="valueConverter"/>.
        /// </summary>
        /// <typeparam name="TExport">The exported item type.</typeparam>
        /// <typeparam name="TProperty">The selected property type.</typeparam>
        /// <typeparam name="TValue">The converted value type written to the cell.</typeparam>
        /// <param name="sheet">The export sheet.</param>
        /// <param name="propertySelector">A property access expression used to locate the existing column and read its value.</param>
        /// <param name="valueConverter">A function that converts the property value to the cell value.</param>
        /// <returns>The same sheet instance.</returns>
        /// <exception cref="InvalidOperationException">The expression is not a property access, or no matching column exists.</exception>
        public static IExcelExportSheet<TExport> OverrideColumn<TExport, TProperty, TValue>(this IExcelExportSheet<TExport> sheet, Expression<Func<TExport, TProperty>> propertySelector, Func<TProperty, TValue> valueConverter)
        {
            var column = GetClrColumn(sheet, propertySelector);
            var reader = propertySelector.Compile();
            var newColumn = column.Override(obj => valueConverter(reader(obj)));
            ReplaceColumn(sheet, column, newColumn);
            return sheet;
        }
    }
}
