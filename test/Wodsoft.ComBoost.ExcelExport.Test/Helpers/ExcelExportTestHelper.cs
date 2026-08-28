using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Wodsoft.ComBoost.ExcelExport.NPOI;
using Wodsoft.ComBoost.ExcelExport.Test.Models;

namespace Wodsoft.ComBoost.ExcelExport.Test.Helpers
{
    internal static class ExcelExportTestHelper
    {
        public static IExcelExportSheet<TExport> KeepClrColumns<TExport>(this IExcelExportSheet<TExport> sheet, params string[] clrNames)
        {
            var columns = new List<IExcelExportColumn<TExport>>(clrNames.Length);
            foreach (var name in clrNames)
                columns.Add(sheet.GetClrColumn(name));
            sheet.Columns.Clear();
            foreach (var column in columns)
                sheet.Columns.Add(column);
            return sheet;
        }

        public static void SetDataFormat<TExport>(this IExcelExportColumn<TExport> column, string format)
        {
            for (int i = column.Features.Count - 1; i >= 0; i--)
            {
                if (column.Features[i] is IExcelExportDataFormatFeature)
                    column.Features.RemoveAt(i);
            }
            column.Features.Add(new ExcelExportDataFormatFeature(format));
        }

        public static IExcelExportSheet<SampleExportItem> CreateBasicSheet(IExcelExportService service, string? name = "Sheet1")
        {
            var sheet = service.GetExportSheet<SampleExportItem>();
            sheet.KeepClrColumns(
                nameof(SampleExportItem.Name),
                nameof(SampleExportItem.Age),
                nameof(SampleExportItem.Score));
            sheet.Name = name;
            sheet.CreateHeaders = true;
            return sheet;
        }

        public static IExcelExportSheet<SampleExportItem> CreateTypedSheet(IExcelExportService service, string? name = "Sheet1")
        {
            var sheet = service.GetExportSheet<SampleExportItem>();
            sheet.KeepClrColumns(
                nameof(SampleExportItem.Name),
                nameof(SampleExportItem.Active),
                nameof(SampleExportItem.Age),
                nameof(SampleExportItem.LongValue),
                nameof(SampleExportItem.DoubleValue),
                nameof(SampleExportItem.DecimalValue),
                nameof(SampleExportItem.CreatedAt));
            sheet.Name = name;
            sheet.CreateHeaders = true;
            return sheet;
        }

        public static XSSFWorkbook ExportToWorkbook(
            NpoiExcelExportService service,
            IExcelExportSheet<SampleExportItem> sheet,
            IEnumerable<SampleExportItem> items,
            NpoiExcelExportContext? context = null)
        {
            return ExportToWorkbook<SampleExportItem>(service, sheet, items, context);
        }

        public static XSSFWorkbook ExportToWorkbook<TExport>(
            NpoiExcelExportService service,
            IExcelExportSheet<TExport> sheet,
            IEnumerable<TExport> items,
            NpoiExcelExportContext? context = null)
        {
            context ??= (NpoiExcelExportContext)service.CreateContext();
            using (context)
            {
                service.Export(context, sheet, items);
                using var ms = new MemoryStream();
                context.Write(ms);
                ms.Position = 0;
                return new XSSFWorkbook(ms);
            }
        }

        public static IExcelExportSheet<SampleOrderExportItem> CreateOrderSheet(IExcelExportService service, bool mergeHeader)
        {
            var sheet = service.GetExportSheet<SampleOrderExportItem>();
            sheet.Name = "Sheet1";
            sheet.CreateHeaders = true;
            var ordersColumn = sheet.GetClrColumn(x => x.Orders);
            Assert.True(ordersColumn.TryGetFeature<IExcelExportExpandingFeature<SampleOrderExportItem>>(out var expanding));
            ((ExcelExportExpandingFeature<SampleOrderExportItem>)expanding!).MergeHeader = mergeHeader;
            return sheet;
        }

        public static async Task<XSSFWorkbook> ExportToWorkbookAsync(
            NpoiExcelExportService service,
            IExcelExportSheet<SampleExportItem> sheet,
            IAsyncEnumerable<SampleExportItem> items)
        {
            using var context = (NpoiExcelExportContext)service.CreateContext();
            await service.ExportAsync(context, sheet, items);
            using var ms = new MemoryStream();
            context.Write(ms);
            ms.Position = 0;
            return new XSSFWorkbook(ms);
        }

        public static string GetStringCell(ISheet sheet, int row, int column)
        {
            var cell = sheet.GetRow(row)?.GetCell(column);
            Assert.NotNull(cell);
            return cell.CellType switch
            {
                CellType.String => cell.StringCellValue,
                CellType.Numeric => cell.NumericCellValue.ToString(),
                CellType.Boolean => cell.BooleanCellValue.ToString(),
                _ => cell.ToString() ?? string.Empty
            };
        }

        public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(IEnumerable<T> source)
        {
            foreach (var item in source)
            {
                yield return item;
                await Task.Yield();
            }
        }
    }
}
