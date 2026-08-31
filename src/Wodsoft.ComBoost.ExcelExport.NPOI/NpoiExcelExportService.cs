using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.Streaming;
using NPOI.XSSF.UserModel;
using Org.BouncyCastle.Asn1.X509;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Wodsoft.ComBoost.ExcelExport.NPOI
{
    /// <summary>
    /// Exports Excel workbooks using NPOI <c>SXSSFWorkbook</c>.
    /// </summary>
    public class NpoiExcelExportService : ExcelExportService
    {
        private const int ValidationEndRow = 1048575;

        private readonly ConcurrentDictionary<Type, Func<NpoiExcelExportService, NpoiExcelExportContext, ISheet, IEnumerable<IExcelExportColumn>, object, int, int, int>> _expandingItemCache = new();
        private readonly ConcurrentDictionary<Type, Func<NpoiExcelExportService, NpoiExcelExportContext, ISheet, IEnumerable<IExcelExportColumn>, int, int, (int, int)>> _expandingHeaderCache = new();
        private readonly ConcurrentDictionary<Type, Func<NpoiExcelExportService, NpoiExcelExportContext, ISheet, IEnumerable<IExcelExportColumn>, int, int, int>> _expandingColumnStyleCache = new();

        /// <inheritdoc />
        public override IExcelExportContext CreateContext(IServiceProvider? services)
        {
            return new NpoiExcelExportContext(services, new NpoiExcelExportSettings());
        }

        /// <inheritdoc />
        public override void Export<TExport>(IExcelExportContext context, IExcelExportSheet<TExport> sheet, IEnumerable<TExport> source)
        {
            if (context is not NpoiExcelExportContext npoiContext)
                throw new ArgumentException("Specified context is not NpoiExcelExportContext. Please use CreateContext to create a NpoiExcelExportContext.");
            ISheet npoiSheet;
            if (sheet.Name == null)
                npoiSheet = npoiContext.WorkBook.CreateSheet();
            else
                npoiSheet = npoiContext.WorkBook.GetSheet(sheet.Name) ?? npoiContext.WorkBook.CreateSheet(sheet.Name);

            int rowIndex = sheet.StartRow;
            if (sheet.CreateHeaders)
            {
                (var rows, _) = BuildHeaders(npoiContext, npoiSheet, sheet.Columns, rowIndex, sheet.StartColumn);
                rowIndex += rows;
            }

            BuildColumnStyles(npoiContext, npoiSheet, sheet.Columns, rowIndex, sheet.StartColumn);

            foreach (var item in source)
            {
                rowIndex += BuildItem(npoiContext, npoiSheet, sheet.Columns, item, rowIndex, sheet.StartColumn);
            }
        }

        /// <inheritdoc />
        public override async Task ExportAsync<TExport>(IExcelExportContext context, IExcelExportSheet<TExport> sheet, IAsyncEnumerable<TExport> source)
        {
            if (context is not NpoiExcelExportContext npoiContext)
                throw new ArgumentException("Specified context is not NpoiExcelExportContext. Please use CreateContext to create a NpoiExcelExportContext.");
            ISheet npoiSheet;
            var sheetName = GetSheetName(npoiContext, sheet);
            if (sheetName == null)
                npoiSheet = npoiContext.WorkBook.CreateSheet();
            else
                npoiSheet = npoiContext.WorkBook.GetSheet(sheetName) ?? npoiContext.WorkBook.CreateSheet(sheetName);

            int rowIndex = sheet.StartRow;
            if (sheet.CreateHeaders)
            {
                (var rows, _) = BuildHeaders(npoiContext, npoiSheet, sheet.Columns, rowIndex, sheet.StartColumn);
                rowIndex += rows;
            }

            BuildColumnStyles(npoiContext, npoiSheet, sheet.Columns, rowIndex, sheet.StartColumn);

            await foreach (var item in source)
            {
                rowIndex += BuildItem(npoiContext, npoiSheet, sheet.Columns, item, rowIndex, sheet.StartColumn);
            }
        }

        /// <summary>
        /// Gets the workbook sheet name used for the export, or <see langword="null"/> to create an unnamed sheet.
        /// </summary>
        /// <typeparam name="TExport">The exported item type.</typeparam>
        /// <param name="context">The NPOI export context.</param>
        /// <param name="sheet">The export sheet definition.</param>
        /// <returns>The sheet name, or <see langword="null"/> to use the workbook default.</returns>
        protected virtual string? GetSheetName<TExport>(NpoiExcelExportContext context, IExcelExportSheet<TExport> sheet) => sheet.Name;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int CalculateWidth(string text)
        {
            return Math.Min(ushort.MaxValue, (text.Sum(t => t < 128 ? 1 : 2) + 2) * 256);
        }

        private (int headerRows, int headerColumns) BuildHeaders<TExport>(NpoiExcelExportContext context, ISheet sheet, IEnumerable<IExcelExportColumn<TExport>> columns, int rowIndex, int columnIndex)
        {
            var row = sheet.GetRow(rowIndex) ?? sheet.CreateRow(rowIndex);
            int x = 0;
            int rows = 1;
            bool hasExpanding = false;
            int expandRows = 0, expandColumns = 0;

            foreach (var column in columns)
            {
                if (column.TryGetFeature<IExcelExportExpandingFeature<TExport>>(out var expandingFeature))
                {
                    if (hasExpanding)
                        throw new NotSupportedException("Exporting object has multiple expanding column.");
                    hasExpanding = true;
                    if (expandingFeature.MergeHeader)
                    {
                        var cell = row.GetCell(columnIndex + x) ?? row.CreateCell(columnIndex + x);
                        BuildHeaderCell(context, cell, column);

                        (expandRows, expandColumns) = BuildExpandingHeaders(context, sheet, expandingFeature.Columns, expandingFeature.ItemType, rowIndex + 1, columnIndex + x);

                        if (expandColumns > 1)
                        {
                            AddMergedRegion(context, sheet, new CellRangeAddress(rowIndex, rowIndex, columnIndex + x, columnIndex + x + expandColumns - 1));
                        }
                        rows = expandRows + 1;
                    }
                    else
                    {
                        (expandRows, expandColumns) = BuildExpandingHeaders(context, sheet, expandingFeature.Columns, expandingFeature.ItemType, rowIndex, columnIndex + x);
                        rows = expandRows;
                    }
                    x += expandColumns;
                }
                else
                {
                    int col = columnIndex + x;
                    var cell = row.GetCell(col) ?? row.CreateCell(col);
                    BuildHeaderCell(context, cell, column);

                    x++;
                }
            }
            if (hasExpanding && rows != 1)
            {
                x = 0;
                foreach (var column in columns)
                {
                    if (column.TryGetFeature<IExcelExportExpandingFeature<TExport>>(out var expandingFeature))
                    {
                        x += expandColumns;
                        continue;
                    }
                    AddMergedRegion(context, sheet, new CellRangeAddress(rowIndex, rowIndex + rows - 1, columnIndex + x, columnIndex + x));
                    x++;
                }
            }

            return (rows, x);
        }

        private static void AddMergedRegion(NpoiExcelExportContext context, ISheet sheet, CellRangeAddress region)
        {
            // SXSSFSheet.AddMergedRegion breaks the streaming writer on dispose;
            // merge on the underlying XSSF sheet instead.
            var xssfSheet = context.WorkBook.XssfWorkbook.GetSheet(sheet.SheetName)
                ?? context.WorkBook.XssfWorkbook.GetSheetAt(context.WorkBook.GetSheetIndex(sheet));
            xssfSheet.AddMergedRegion(region);
        }

        private static readonly MethodInfo BuildHeadersMethod = typeof(NpoiExcelExportService).GetMethod("BuildHeaders", BindingFlags.Instance | BindingFlags.NonPublic)!;

        private (int rows, int columns) BuildExpandingHeaders(NpoiExcelExportContext context, ISheet sheet, IEnumerable<IExcelExportColumn> columns, Type itemType, int rowIndex, int columnIndex)
        {
            var func = _expandingHeaderCache.GetOrAdd(itemType, type =>
            {
                var serviceParameter = Expression.Parameter(typeof(NpoiExcelExportService));
                var contextParameter = Expression.Parameter(typeof(NpoiExcelExportContext));
                var sheetParameter = Expression.Parameter(typeof(ISheet));
                var columnsParameter = Expression.Parameter(typeof(IEnumerable<IExcelExportColumn>));
                var rowIndexParameter = Expression.Parameter(typeof(int));
                var columnIndexParameter = Expression.Parameter(typeof(int));
                var buildHeadersMethod = BuildHeadersMethod.MakeGenericMethod(type);
                var typedColumnType = typeof(IExcelExportColumn<>).MakeGenericType(type);
                var castMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast))!.MakeGenericMethod(typedColumnType);
                var castColumns = Expression.Call(castMethod, columnsParameter);
                var expression = Expression.Call(
                    serviceParameter,
                    buildHeadersMethod,
                    contextParameter,
                    sheetParameter,
                    castColumns,
                    rowIndexParameter,
                    columnIndexParameter);
                return Expression.Lambda<Func<NpoiExcelExportService, NpoiExcelExportContext, ISheet, IEnumerable<IExcelExportColumn>, int, int, (int, int)>>(
                    expression,
                    serviceParameter,
                    contextParameter,
                    sheetParameter,
                    columnsParameter,
                    rowIndexParameter,
                    columnIndexParameter).Compile();
            });
            return func(this, context, sheet, columns, rowIndex, columnIndex);
        }

        /// <summary>
        /// Applies header styling and comments to a header cell.
        /// </summary>
        /// <param name="context">The NPOI export context.</param>
        /// <param name="cell">The header cell.</param>
        /// <param name="column">The column being written.</param>
        protected virtual void BuildHeaderCell<TExport>(NpoiExcelExportContext context, ICell cell, IExcelExportColumn<TExport> column)
        {
            cell.SetCellValue(column.Name);
            if (column.Width.HasValue)
                cell.Row.Sheet.SetColumnWidth(cell.ColumnIndex, column.Width.Value);
            else if (!string.IsNullOrEmpty(column.Name))
                cell.Row.Sheet.SetColumnWidth(cell.ColumnIndex, CalculateWidth(column.Name));
            if (column.TryGetFeature<IExcelExportColorFeature>(out var colorFeature))
            {
                if (colorFeature.HasHeaderBackground || colorFeature.HasHeaderForeground)
                {
                    var style = context.GetStyle(colorFeature.HeaderBackground, colorFeature.HeaderForeground);
                    if (style != null)
                        cell.CellStyle = style;
                }
            }
            if (column.TryGetFeature<IExcelExportCommentFeature>(out var commentFeature))
            {
                if (commentFeature.HeaderComment != null)
                {
                    var helper = context.WorkBook.XssfWorkbook.GetCreationHelper();
                    var drawing = cell.Row.Sheet.CreateDrawingPatriarch();
                    var anchor = helper.CreateClientAnchor();
                    anchor.Col1 = cell.ColumnIndex;
                    anchor.Col2 = cell.ColumnIndex + 1;
                    anchor.Row1 = cell.RowIndex;
                    anchor.Row2 = cell.RowIndex + 1;
                    var comment = (XSSFComment)drawing.CreateCellComment(anchor);
                    comment.String = helper.CreateRichTextString(commentFeature.HeaderComment);
                    comment.Address = new CellAddress(cell.RowIndex, cell.ColumnIndex);
                    cell.CellComment = comment;
                }
            }
        }

        private int BuildColumnStyles<TExport>(NpoiExcelExportContext context, ISheet sheet, IEnumerable<IExcelExportColumn<TExport>> columns, int dataStartRow, int columnIndex)
        {
            int x = 0;
            foreach (var column in columns)
            {
                if (column.TryGetFeature<IExcelExportExpandingFeature<TExport>>(out var expandingFeature))
                {
                    x += BuildExpandingColumnStyles(context, sheet, expandingFeature.Columns, expandingFeature.ItemType, dataStartRow, columnIndex + x);
                    continue;
                }
                BuildColumnStyle(context, sheet, column, dataStartRow, columnIndex + x, null);
                x++;
            }
            return x;
        }

        private static readonly MethodInfo BuildColumnStylesMethod = typeof(NpoiExcelExportService)
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(m => m.Name == nameof(BuildColumnStyles) && m.IsGenericMethodDefinition);

        private int BuildExpandingColumnStyles(NpoiExcelExportContext context, ISheet sheet, IEnumerable<IExcelExportColumn> columns, Type itemType, int dataStartRow, int columnIndex)
        {
            var func = _expandingColumnStyleCache.GetOrAdd(itemType, type =>
            {
                var serviceParameter = Expression.Parameter(typeof(NpoiExcelExportService));
                var contextParameter = Expression.Parameter(typeof(NpoiExcelExportContext));
                var sheetParameter = Expression.Parameter(typeof(ISheet));
                var columnsParameter = Expression.Parameter(typeof(IEnumerable<IExcelExportColumn>));
                var dataStartRowParameter = Expression.Parameter(typeof(int));
                var columnIndexParameter = Expression.Parameter(typeof(int));
                var method = BuildColumnStylesMethod.MakeGenericMethod(type);
                var typedColumnType = typeof(IExcelExportColumn<>).MakeGenericType(type);
                var castMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast))!.MakeGenericMethod(typedColumnType);
                var castColumns = Expression.Call(castMethod, columnsParameter);
                var expression = Expression.Call(
                    serviceParameter,
                    method,
                    contextParameter,
                    sheetParameter,
                    castColumns,
                    dataStartRowParameter,
                    columnIndexParameter);
                return Expression.Lambda<Func<NpoiExcelExportService, NpoiExcelExportContext, ISheet, IEnumerable<IExcelExportColumn>, int, int, int>>(
                    expression,
                    serviceParameter,
                    contextParameter,
                    sheetParameter,
                    columnsParameter,
                    dataStartRowParameter,
                    columnIndexParameter).Compile();
            });
            return func(this, context, sheet, columns, dataStartRow, columnIndex);
        }

        /// <summary>
        /// Applies column-level styles such as data format, colors, and validation.
        /// </summary>
        /// <typeparam name="TExport">The exported item type.</typeparam>
        /// <param name="context">The NPOI export context.</param>
        /// <param name="sheet">The NPOI sheet.</param>
        /// <param name="column">The column being styled.</param>
        /// <param name="dataStartRow">The first data row index.</param>
        /// <param name="columnIndex">The Excel column index.</param>
        /// <param name="style">An existing style to extend, or <see langword="null"/> to create one when needed.</param>
        protected virtual void BuildColumnStyle<TExport>(NpoiExcelExportContext context, ISheet sheet, IExcelExportColumn<TExport> column, int dataStartRow, int columnIndex, XSSFCellStyle? style)
        {
            if (column.TryGetFeature<IExcelExportValidationFeature>(out var validationFeature)
                && validationFeature.Validations.Count > 0)
            {
                var helper = sheet.GetDataValidationHelper();
                var values = validationFeature.Validations as string[] ?? validationFeature.Validations.ToArray();
                var constraint = helper.CreateExplicitListConstraint(FormatValidations(context, sheet, column, values));
                var addressList = new CellRangeAddressList(dataStartRow, ValidationEndRow, columnIndex, columnIndex);
                var validation = helper.CreateValidation(constraint, addressList);
                validation.SuppressDropDownArrow = true;
                sheet.AddValidationData(validation);
            }
            if (column.TryGetFeature<IExcelExportDataFormatFeature>(out var dataFormatFeature) && dataFormatFeature.DataFormat != null)
            {
                style = (XSSFCellStyle)sheet.Workbook.CreateCellStyle();
                style.DataFormat = sheet.Workbook.CreateDataFormat().GetFormat(GetDataFormat<TExport>(context, sheet, column, dataFormatFeature.DataFormat));
            }
            if (column.TryGetFeature<IExcelExportColorFeature>(out var colorFeature))
            {
                if (colorFeature.HasContentBackground || colorFeature.HasContentForeground)
                {
                    style = context.GetStyle(colorFeature.ContentBackground, colorFeature.ContentForeground, style);
                }
            }
            if (style != null)
                sheet.SetDefaultColumnStyle(columnIndex, style);
        }

        /// <summary>
        /// Formats the values written to a column's data validation list.
        /// </summary>
        /// <typeparam name="TExport">The exported item type.</typeparam>
        /// <param name="context">The NPOI export context.</param>
        /// <param name="sheet">The NPOI sheet.</param>
        /// <param name="column">The column that owns the validation.</param>
        /// <param name="sources">The raw validation values from the column feature.</param>
        /// <returns>The values written to the Excel list constraint.</returns>
        protected virtual string[] FormatValidations<TExport>(NpoiExcelExportContext context, ISheet sheet, IExcelExportColumn<TExport> column, string[] sources) => sources;

        /// <summary>
        /// Gets the Excel data format string applied to a column.
        /// </summary>
        /// <typeparam name="TExport">The exported item type.</typeparam>
        /// <param name="context">The NPOI export context.</param>
        /// <param name="sheet">The NPOI sheet.</param>
        /// <param name="column">The column being formatted.</param>
        /// <param name="dataFormat">The data format from the column feature.</param>
        /// <returns>The Excel data format string.</returns>
        protected virtual string GetDataFormat<TExport>(NpoiExcelExportContext context, ISheet sheet, IExcelExportColumn<TExport> column, string dataFormat) => dataFormat;

        private int BuildItem<TExport>(NpoiExcelExportContext context, ISheet sheet, IEnumerable<IExcelExportColumn<TExport>> columns, TExport item, int rowIndex, int columnIndex)
        {
            var row = sheet.GetRow(rowIndex) ?? sheet.CreateRow(rowIndex);
            int rows = 1;
            int x = 0;
            bool hasExpanding = false;
            foreach (var column in columns)
            {
                if (column.TryGetFeature<IExcelExportExpandingFeature<TExport>>(out var expandingFeature))
                {
                    if (hasExpanding)
                        throw new NotSupportedException("Exporting object has multiple expanding column.");
                    hasExpanding = true;
                    var expandingItems = expandingFeature.GetItems(item);
                    int expandingRow = 0;
                    foreach (var expandingItem in expandingItems)
                    {
                        expandingRow += BuildExpandingItem(context, sheet, expandingFeature.Columns, expandingFeature.ItemType, expandingItem, rowIndex + expandingRow, columnIndex + x);
                    }
                    if (expandingRow != 0)
                        rows = expandingRow;
                    x += expandingFeature.Columns.Count;
                }
                else
                {
                    var cell = row.GetCell(x + columnIndex) ?? row.CreateCell(x + columnIndex);
                    //var sheetColumn = ((SXSSFSheet)sheet).GetColumn(x + columnIndex);
                    //if (sheetColumn != null && cell.CellStyle != sheetColumn.ColumnStyle)
                    //    cell.CellStyle = sheetColumn.ColumnStyle;
                    BuildItemCell(context, cell, column, item);
                    x++;
                }
            }
            if (hasExpanding)
            {
                x = 0;
                foreach (var column in columns)
                {
                    if (column.TryGetFeature<IExcelExportExpandingFeature<TExport>>(out var expandingFeature))
                    {
                        x += expandingFeature.Columns.Count;
                        continue;
                    }
                    var cell = row.GetCell(x + columnIndex);
                    if (cell != null)
                    {
                        for (int copyRowIndex = rowIndex + 1; copyRowIndex < rowIndex + rows; copyRowIndex++)
                        {
                            var copyRow = sheet.GetRow(copyRowIndex) ?? sheet.CreateRow(copyRowIndex);
                            var copyCell = copyRow.CreateCell(x + columnIndex);
                            switch (cell.CellType)
                            {
                                case CellType.String:
                                    copyCell.SetCellValue(cell.StringCellValue);
                                    break;
                                case CellType.Numeric:
                                    copyCell.SetCellValue(cell.NumericCellValue);
                                    break;
                                case CellType.Boolean:
                                    copyCell.SetCellValue(cell.BooleanCellValue);
                                    break;
                                case CellType.Formula:
                                    copyCell.SetCellFormula(cell.CellFormula);
                                    break;
                                case CellType.Error:
                                    copyCell.SetCellErrorValue(cell.ErrorCellValue);
                                    break;
                                case CellType.Blank:
                                    copyCell.SetBlank();
                                    break;
                            }
                            if (cell.CellStyle != null)
                                copyCell.CellStyle = cell.CellStyle;
                        }
                    }
                    x++;
                }
            }
            return rows;
        }

        private static readonly MethodInfo BuildItemMethod = typeof(NpoiExcelExportService)
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(m => m.Name == nameof(BuildItem) && m.IsGenericMethodDefinition);

        private int BuildExpandingItem(NpoiExcelExportContext context, ISheet sheet, IEnumerable<IExcelExportColumn> columns, Type itemType, object item, int rowIndex, int columnIndex)
        {
            var func = _expandingItemCache.GetOrAdd(itemType, type =>
            {
                var serviceParameter = Expression.Parameter(typeof(NpoiExcelExportService));
                var contextParameter = Expression.Parameter(typeof(NpoiExcelExportContext));
                var sheetParameter = Expression.Parameter(typeof(ISheet));
                var columnsParameter = Expression.Parameter(typeof(IEnumerable<IExcelExportColumn>));
                var itemParameter = Expression.Parameter(typeof(object));
                var rowIndexParameter = Expression.Parameter(typeof(int));
                var columnIndexParameter = Expression.Parameter(typeof(int));
                var buildItemMethod = BuildItemMethod.MakeGenericMethod(type);
                var typedColumnType = typeof(IExcelExportColumn<>).MakeGenericType(type);
                var castMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast))!.MakeGenericMethod(typedColumnType);
                var castColumns = Expression.Call(castMethod, columnsParameter);
                var expression = Expression.Call(
                    serviceParameter,
                    buildItemMethod,
                    contextParameter,
                    sheetParameter,
                    castColumns,
                    Expression.Convert(itemParameter, type),
                    rowIndexParameter,
                    columnIndexParameter);
                return Expression.Lambda<Func<NpoiExcelExportService, NpoiExcelExportContext, ISheet, IEnumerable<IExcelExportColumn>, object, int, int, int>>(
                    expression,
                    serviceParameter,
                    contextParameter,
                    sheetParameter,
                    columnsParameter,
                    itemParameter,
                    rowIndexParameter,
                    columnIndexParameter).Compile();
            });
            return func(this, context, sheet, columns, item, rowIndex, columnIndex);
        }

        /// <summary>
        /// Writes a data cell using the registered cell writer for the column type.
        /// </summary>
        /// <typeparam name="TExport">The exported item type.</typeparam>
        /// <param name="context">The NPOI export context.</param>
        /// <param name="cell">The data cell.</param>
        /// <param name="column">The column being written.</param>
        /// <param name="item">The exported item.</param>
        protected virtual void BuildItemCell<TExport>(NpoiExcelExportContext context, ICell cell, IExcelExportColumn<TExport> column, TExport item)
        {
            INpoiExcelExportCellWriter? writer;
            var valueType = Nullable.GetUnderlyingType(column.Type);
            if (valueType == null)
            {
                if (!context.Settings.TryGetWriter(column.Type, out writer))
                    throw new NotSupportedException($"No writer of type \"{column.Type.FullName}\" found.");
            }
            else
            {
                if (!context.Settings.TryGetWriter(valueType, out writer))
                    throw new NotSupportedException($"No writer of type \"{valueType.FullName}\" found.");
            }
            writer.Write(this, context, column, cell, item);
            if (context.Settings.CreateCellForNullValue == false && cell.CellType == CellType.Blank)
                cell.Row.RemoveCell(cell);
        }

        /// <summary>
        /// Writes a boolean value to the specified cell.
        /// </summary>
        /// <typeparam name="TExport">The exported item type.</typeparam>
        /// <param name="context">The NPOI export context.</param>
        /// <param name="cell">The target cell.</param>
        /// <param name="column">The column being written.</param>
        /// <param name="item">The exported item.</param>
        /// <param name="value">The boolean value.</param>
        public virtual void WriteCell<TExport>(NpoiExcelExportContext context, ICell cell, IExcelExportColumn<TExport> column, TExport item, bool value)
        {
            cell.SetCellValue(value);
        }

        /// <summary>
        /// Writes a date and time value to the specified cell.
        /// </summary>
        /// <typeparam name="TExport">The exported item type.</typeparam>
        /// <param name="context">The NPOI export context.</param>
        /// <param name="cell">The target cell.</param>
        /// <param name="column">The column being written.</param>
        /// <param name="item">The exported item.</param>
        /// <param name="value">The date and time value.</param>
        public virtual void WriteCell<TExport>(NpoiExcelExportContext context, ICell cell, IExcelExportColumn<TExport> column, TExport item, DateTime value)
        {
            cell.SetCellValue(value);
        }

        /// <summary>
        /// Writes a numeric value to the specified cell.
        /// </summary>
        /// <typeparam name="TExport">The exported item type.</typeparam>
        /// <param name="context">The NPOI export context.</param>
        /// <param name="cell">The target cell.</param>
        /// <param name="column">The column being written.</param>
        /// <param name="item">The exported item.</param>
        /// <param name="value">The numeric value.</param>
        public virtual void WriteCell<TExport>(NpoiExcelExportContext context, ICell cell, IExcelExportColumn<TExport> column, TExport item, double value)
        {
            cell.SetCellValue(value);
        }

        /// <summary>
        /// Writes a text value to the specified cell.
        /// </summary>
        /// <typeparam name="TExport">The exported item type.</typeparam>
        /// <param name="context">The NPOI export context.</param>
        /// <param name="cell">The target cell.</param>
        /// <param name="column">The column being written.</param>
        /// <param name="item">The exported item.</param>
        /// <param name="value">The text value.</param>
        public virtual void WriteCell<TExport>(NpoiExcelExportContext context, ICell cell, IExcelExportColumn<TExport> column, TExport item, string value)
        {
            cell.SetCellValue(value);
        }

        /// <summary>
        /// Writes rich text to the specified cell.
        /// </summary>
        /// <typeparam name="TExport">The exported item type.</typeparam>
        /// <param name="context">The NPOI export context.</param>
        /// <param name="cell">The target cell.</param>
        /// <param name="column">The column being written.</param>
        /// <param name="item">The exported item.</param>
        /// <param name="value">The rich text value.</param>
        public virtual void WriteCell<TExport>(NpoiExcelExportContext context, ICell cell, IExcelExportColumn<TExport> column, TExport item, IRichTextString value)
        {
            cell.SetCellValue(value);
        }
    }
}
