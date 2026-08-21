using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Wodsoft.ComBoost.ExcelExport.NPOI;
using Wodsoft.ComBoost.ExcelExport.Test.Models;

namespace Wodsoft.ComBoost.ExcelExport.Test.Helpers
{
    internal static class ExcelExportTestHelper
    {
        public static ExcelExportSheet<SampleExportItem> CreateBasicSheet(string? name = "Sheet1")
        {
            return new ExcelExportSheet<SampleExportItem>(name, new List<IExcelExportColumn<SampleExportItem>>
            {
                new ExcelExportColumn<SampleExportItem, string?>("Name", null, x => x.Name),
                new ExcelExportColumn<SampleExportItem, int>("Age", null, x => x.Age),
                new ExcelExportColumn<SampleExportItem, int?>("Score", null, x => x.Score),
            })
            {
                CreateHeaders = true
            };
        }

        public static ExcelExportSheet<SampleExportItem> CreateTypedSheet(string? name = "Sheet1")
        {
            return new ExcelExportSheet<SampleExportItem>(name, new List<IExcelExportColumn<SampleExportItem>>
            {
                new ExcelExportColumn<SampleExportItem, string?>("Name", null, x => x.Name),
                new ExcelExportColumn<SampleExportItem, bool>("Active", null, x => x.Active),
                new ExcelExportColumn<SampleExportItem, int>("Age", null, x => x.Age),
                new ExcelExportColumn<SampleExportItem, long>("LongValue", null, x => x.LongValue),
                new ExcelExportColumn<SampleExportItem, double>("DoubleValue", null, x => x.DoubleValue),
                new ExcelExportColumn<SampleExportItem, decimal>("DecimalValue", null, x => x.DecimalValue),
                new ExcelExportColumn<SampleExportItem, DateTime>("CreatedAt", null, x => x.CreatedAt),
            })
            {
                CreateHeaders = true
            };
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

        public static ExcelExportSheet<SampleOrderExportItem> CreateOrderSheet(bool mergeHeader)
        {
            var orderColumns = new List<IExcelExportColumn>
            {
                new ExcelExportColumn<SampleOrderLine, int>("Id", null, x => x.Id),
                new ExcelExportColumn<SampleOrderLine, int>("Qty", null, x => x.Qty),
            };
            var ordersColumn = new ExcelExportColumn<SampleOrderExportItem, List<SampleOrderLine>>("Orders", null, x => x.Orders);
            ordersColumn.Features.Add(new ExcelExportExpandingFeature<SampleOrderExportItem>(
                typeof(SampleOrderLine),
                orderColumns,
                x => x.Orders)
            {
                MergeHeader = mergeHeader
            });

            return new ExcelExportSheet<SampleOrderExportItem>("Sheet1", new List<IExcelExportColumn<SampleOrderExportItem>>
            {
                new ExcelExportColumn<SampleOrderExportItem, string?>("Name", null, x => x.Name),
                ordersColumn,
                new ExcelExportColumn<SampleOrderExportItem, int>("Age", null, x => x.Age),
            })
            {
                CreateHeaders = true
            };
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