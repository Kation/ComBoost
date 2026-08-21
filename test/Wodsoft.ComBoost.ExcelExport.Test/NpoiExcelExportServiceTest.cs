using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using Wodsoft.ComBoost.ExcelExport.NPOI;
using Wodsoft.ComBoost.ExcelExport.Test.Helpers;
using Wodsoft.ComBoost.ExcelExport.Test.Models;

namespace Wodsoft.ComBoost.ExcelExport.Test
{
    public class NpoiExcelExportServiceTest
    {
        private readonly NpoiExcelExportService _service = new();

        [Fact]
        public void CreateContext_ReturnsNpoiContextWithDefaultSettings()
        {
            using var context = (NpoiExcelExportContext)_service.CreateContext();

            Assert.NotNull(context);
            Assert.False(context.Settings.CreateCellForNullValue);
            Assert.True(context.Settings.TryGetWriter(typeof(string), out _));
            Assert.True(context.Settings.TryGetWriter(typeof(int), out _));
            Assert.True(context.Settings.TryGetWriter(typeof(DateTime), out _));
            Assert.True(context.Settings.TryGetWriter(typeof(SampleStatus), out _));
        }

        [Fact]
        public void Export_ThrowsWhenContextIsNotNpoi()
        {
            var sheet = ExcelExportTestHelper.CreateBasicSheet();
            var fakeContext = new FakeExcelExportContext();

            var ex = Assert.Throws<ArgumentException>(() =>
                _service.Export(fakeContext, sheet, Array.Empty<SampleExportItem>()));

            Assert.Contains("NpoiExcelExportContext", ex.Message);
        }

        [Fact]
        public async Task ExportAsync_ThrowsWhenContextIsNotNpoi()
        {
            var sheet = ExcelExportTestHelper.CreateBasicSheet();
            var fakeContext = new FakeExcelExportContext();

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.ExportAsync(fakeContext, sheet, ExcelExportTestHelper.ToAsyncEnumerable(Array.Empty<SampleExportItem>())));

            Assert.Contains("NpoiExcelExportContext", ex.Message);
        }

        [Fact]
        public void Export_WithHeaders_WritesHeaderAndDataRows()
        {
            var sheet = ExcelExportTestHelper.CreateBasicSheet("People");
            var items = new[]
            {
                new SampleExportItem { Name = "Alice", Age = 30, Score = 90 },
                new SampleExportItem { Name = "Bob", Age = 25, Score = 80 },
            };

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(_service, sheet, items);
            var npoiSheet = workbook.GetSheet("People");

            Assert.NotNull(npoiSheet);
            Assert.Equal("Name", ExcelExportTestHelper.GetStringCell(npoiSheet, 0, 0));
            Assert.Equal("Age", ExcelExportTestHelper.GetStringCell(npoiSheet, 0, 1));
            Assert.Equal("Score", ExcelExportTestHelper.GetStringCell(npoiSheet, 0, 2));
            Assert.Equal("Alice", ExcelExportTestHelper.GetStringCell(npoiSheet, 1, 0));
            Assert.Equal(30, npoiSheet.GetRow(1)!.GetCell(1)!.NumericCellValue);
            Assert.Equal(90, npoiSheet.GetRow(1)!.GetCell(2)!.NumericCellValue);
            Assert.Equal("Bob", ExcelExportTestHelper.GetStringCell(npoiSheet, 2, 0));
        }

        [Fact]
        public void Export_WithoutHeaders_StartsWithData()
        {
            var sheet = ExcelExportTestHelper.CreateBasicSheet();
            sheet.CreateHeaders = false;
            var items = new[] { new SampleExportItem { Name = "Alice", Age = 30, Score = 90 } };

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(_service, sheet, items);
            var npoiSheet = workbook.GetSheet("Sheet1");

            Assert.Equal("Alice", ExcelExportTestHelper.GetStringCell(npoiSheet, 0, 0));
            Assert.Null(npoiSheet.GetRow(1));
        }

        [Fact]
        public void Export_WithNullSheetName_CreatesDefaultSheet()
        {
            var sheet = ExcelExportTestHelper.CreateBasicSheet(null);
            var items = new[] { new SampleExportItem { Name = "Alice", Age = 1, Score = 1 } };

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(_service, sheet, items);

            Assert.Equal(1, workbook.NumberOfSheets);
            Assert.Equal("Alice", ExcelExportTestHelper.GetStringCell(workbook.GetSheetAt(0), 1, 0));
        }

        [Fact]
        public void Export_RespectsStartRowAndStartColumn()
        {
            var sheet = ExcelExportTestHelper.CreateBasicSheet();
            sheet.StartRow = 2;
            sheet.StartColumn = 1;
            var items = new[] { new SampleExportItem { Name = "Alice", Age = 30, Score = 90 } };

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(_service, sheet, items);
            var npoiSheet = workbook.GetSheet("Sheet1");

            Assert.Equal("Name", ExcelExportTestHelper.GetStringCell(npoiSheet, 2, 1));
            Assert.Equal("Alice", ExcelExportTestHelper.GetStringCell(npoiSheet, 3, 1));
            Assert.Equal(30, npoiSheet.GetRow(3)!.GetCell(2)!.NumericCellValue);
        }

        [Fact]
        public void Export_EmptySource_WritesHeadersOnly()
        {
            var sheet = ExcelExportTestHelper.CreateBasicSheet();

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(_service, sheet, Array.Empty<SampleExportItem>());
            var npoiSheet = workbook.GetSheet("Sheet1");

            Assert.Equal("Name", ExcelExportTestHelper.GetStringCell(npoiSheet, 0, 0));
            Assert.Null(npoiSheet.GetRow(1));
        }

        [Fact]
        public async Task ExportAsync_MatchesSyncResults()
        {
            var sheet = ExcelExportTestHelper.CreateBasicSheet("Async");
            var items = new[]
            {
                new SampleExportItem { Name = "Alice", Age = 30, Score = 90 },
                new SampleExportItem { Name = "Bob", Age = 25, Score = 80 },
            };

            using var syncWorkbook = ExcelExportTestHelper.ExportToWorkbook(_service, sheet, items);
            using var asyncWorkbook = await ExcelExportTestHelper.ExportToWorkbookAsync(
                _service, sheet, ExcelExportTestHelper.ToAsyncEnumerable(items));

            var syncSheet = syncWorkbook.GetSheet("Async");
            var asyncSheet = asyncWorkbook.GetSheet("Async");

            Assert.Equal(
                ExcelExportTestHelper.GetStringCell(syncSheet, 1, 0),
                ExcelExportTestHelper.GetStringCell(asyncSheet, 1, 0));
            Assert.Equal(
                syncSheet.GetRow(2)!.GetCell(1)!.NumericCellValue,
                asyncSheet.GetRow(2)!.GetCell(1)!.NumericCellValue);
        }

        [Fact]
        public void Export_WritesRepresentativeTypes()
        {
            var createdAt = new DateTime(2024, 5, 6, 12, 30, 0, DateTimeKind.Unspecified);
            var sheet = ExcelExportTestHelper.CreateTypedSheet();
            var items = new[]
            {
                new SampleExportItem
                {
                    Name = "Alice",
                    Active = true,
                    Age = 30,
                    LongValue = 123456789L,
                    DoubleValue = 1.5,
                    DecimalValue = 9.99m,
                    CreatedAt = createdAt,
                }
            };

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(_service, sheet, items);
            var npoiSheet = workbook.GetSheet("Sheet1");
            var row = npoiSheet.GetRow(1)!;

            Assert.Equal("Alice", row.GetCell(0)!.StringCellValue);
            Assert.True(row.GetCell(1)!.BooleanCellValue);
            Assert.Equal(30, row.GetCell(2)!.NumericCellValue);
            Assert.Equal(123456789L, (long)row.GetCell(3)!.NumericCellValue);
            Assert.Equal(1.5, row.GetCell(4)!.NumericCellValue);
            Assert.Equal(9.99, row.GetCell(5)!.NumericCellValue);
            Assert.Equal(createdAt, row.GetCell(6)!.DateCellValue);
        }

        [Fact]
        public void Export_NullValues_RemovesCellByDefault()
        {
            var sheet = ExcelExportTestHelper.CreateBasicSheet();
            var items = new[] { new SampleExportItem { Name = null, Age = 30, Score = null } };

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(_service, sheet, items);
            var row = workbook.GetSheet("Sheet1").GetRow(1)!;

            Assert.Null(row.GetCell(0));
            Assert.Equal(30, row.GetCell(1)!.NumericCellValue);
            Assert.Null(row.GetCell(2));
        }

        [Fact]
        public void Export_NullValues_KeepsBlankCellWhenConfigured()
        {
            var settings = new NpoiExcelExportSettings { CreateCellForNullValue = true };
            var context = new NpoiExcelExportContext(null, settings);
            var sheet = ExcelExportTestHelper.CreateBasicSheet();
            var items = new[] { new SampleExportItem { Name = null, Age = 30, Score = null } };

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(_service, sheet, items, context);
            var row = workbook.GetSheet("Sheet1").GetRow(1)!;

            Assert.NotNull(row.GetCell(0));
            Assert.Equal(CellType.Blank, row.GetCell(0)!.CellType);
            Assert.NotNull(row.GetCell(2));
            Assert.Equal(CellType.Blank, row.GetCell(2)!.CellType);
        }

        [Fact]
        public void Export_UnsupportedType_ThrowsNotSupportedException()
        {
            var sheet = new ExcelExportSheet<SampleExportItem>("Sheet1", new List<IExcelExportColumn<SampleExportItem>>
            {
                new ExcelExportColumn<SampleExportItem, Guid>("Id", null, _ => Guid.NewGuid()),
            })
            {
                CreateHeaders = true
            };

            var context = (NpoiExcelExportContext)_service.CreateContext();
            try
            {
                Assert.Throws<NotSupportedException>(() =>
                    _service.Export(context, sheet, new[] { new SampleExportItem() }));
            }
            finally
            {
                try { context.Dispose(); }
                catch (ObjectDisposedException) { }
                catch (IOException) { }
            }
        }

        [Fact]
        public void Export_ColumnWidth_UsesExplicitAndAutoWidth()
        {
            var columns = new List<IExcelExportColumn<SampleExportItem>>
            {
                new ExcelExportColumn<SampleExportItem, string?>("Name", null, x => x.Name) { Width = 4000 },
                new ExcelExportColumn<SampleExportItem, int>("Age", null, x => x.Age),
            };
            var sheet = new ExcelExportSheet<SampleExportItem>("Sheet1", columns) { CreateHeaders = true };
            var items = new[] { new SampleExportItem { Name = "Alice", Age = 30 } };

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(_service, sheet, items);
            var npoiSheet = workbook.GetSheet("Sheet1");

            Assert.Equal(4000, npoiSheet.GetColumnWidth(0));
            Assert.True(npoiSheet.GetColumnWidth(1) > 0);
        }

        [Fact]
        public void Export_ColorFeature_AppliesHeaderStyle()
        {
            var sheet = ExcelExportTestHelper.CreateBasicSheet();
            sheet.Columns[0].Features.Add(new ExcelExportColorFeature(
                headerBackground: [0xFF, 0x00, 0x00],
                headerForeground: [0xFF, 0xFF, 0xFF],
                contentBackground: null,
                contentForeground: null));

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(
                _service, sheet, new[] { new SampleExportItem { Name = "Alice", Age = 1, Score = 1 } });
            var headerCell = workbook.GetSheet("Sheet1").GetRow(0)!.GetCell(0)!;

            Assert.NotNull(headerCell.CellStyle);
            Assert.NotEqual(0, headerCell.CellStyle.Index);
        }

        [Fact]
        public void Export_ColumnFormat_WritesValueWithExcelDataFormat()
        {
            var createdAt = new DateTime(2024, 5, 6, 12, 30, 0, DateTimeKind.Unspecified);
            var sheet = new ExcelExportSheet<SampleExportItem>("Sheet1", new List<IExcelExportColumn<SampleExportItem>>
            {
                new ExcelExportColumn<SampleExportItem, DateTime>("CreatedAt", null, x => x.CreatedAt),
                new ExcelExportColumn<SampleExportItem, decimal>("DecimalValue", null, x => x.DecimalValue),
            })
            {
                CreateHeaders = true
            };
            sheet.Columns[0].Features.Add(new ExcelExportDataFormatFeature("yyyy-mm-dd"));
            sheet.Columns[1].Features.Add(new ExcelExportDataFormatFeature("0.00"));

            var items = new[]
            {
                new SampleExportItem { CreatedAt = createdAt, DecimalValue = 9.9m }
            };

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(_service, sheet, items);
            var npoiSheet = workbook.GetSheet("Sheet1");
            var row = npoiSheet.GetRow(1)!;
            var dateCell = row.GetCell(0)!;
            var decimalCell = row.GetCell(1)!;

            Assert.Equal(CellType.Numeric, dateCell.CellType);
            Assert.Equal(createdAt, dateCell.DateCellValue);
            Assert.Equal(CellType.Numeric, decimalCell.CellType);
            Assert.Equal(9.9, decimalCell.NumericCellValue, 3);

            Assert.Equal("yyyy-mm-dd", GetColumnDataFormat(npoiSheet, 0));
            Assert.Equal("0.00", GetColumnDataFormat(npoiSheet, 1));
        }

        [Fact]
        public void Export_EnumWriter_WritesDisplayName()
        {
            var sheet = new ExcelExportSheet<SampleExportItem>("Sheet1", new List<IExcelExportColumn<SampleExportItem>>
            {
                new ExcelExportColumn<SampleExportItem, SampleStatus>("Status", null, x => x.Status),
            })
            {
                CreateHeaders = true
            };

            var items = new[]
            {
                new SampleExportItem { Status = SampleStatus.Published },
                new SampleExportItem { Status = SampleStatus.Archived },
            };

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(_service, sheet, items);
            var npoiSheet = workbook.GetSheet("Sheet1");

            Assert.Equal("已发布", npoiSheet.GetRow(1)!.GetCell(0)!.StringCellValue);
            Assert.Equal("Archived", npoiSheet.GetRow(2)!.GetCell(0)!.StringCellValue);
        }

        [Fact]
        public void Export_CommentFeature_WritesHeaderComment()
        {
            var sheet = ExcelExportTestHelper.CreateBasicSheet();
            sheet.Columns[0].Features.Add(new ExcelExportCommentFeature("备注说明"));

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(
                _service, sheet, new[] { new SampleExportItem { Name = "Alice", Age = 1, Score = 1 } });
            var npoiSheet = workbook.GetSheet("Sheet1");
            var comment = npoiSheet.GetCellComment(new CellAddress(0, 0));

            Assert.NotNull(comment);
            Assert.Equal("备注说明", comment.String.String);
        }

        [Fact]
        public void Export_ValidationFeature_AddsDataValidation()
        {
            var sheet = ExcelExportTestHelper.CreateBasicSheet();
            sheet.Columns[0].Features.Add(new ExcelExportValidationFeature(["A", "B", "C"]));

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(
                _service, sheet, new[] { new SampleExportItem { Name = "A", Age = 1, Score = 1 } });
            var npoiSheet = workbook.GetSheet("Sheet1");
            var validations = npoiSheet.GetDataValidations();

            Assert.NotEmpty(validations);
            var constraint = validations[0].ValidationConstraint;
            Assert.Equal(ValidationType.LIST, constraint.GetValidationType());
            Assert.Contains("A", constraint.ExplicitListValues);
            Assert.Contains("B", constraint.ExplicitListValues);
            Assert.Contains("C", constraint.ExplicitListValues);
        }

        [Fact]
        public void Export_EnumValidationFeature_AddsDataValidation()
        {
            var sheet = new ExcelExportSheet<SampleExportItem>("Sheet1", new List<IExcelExportColumn<SampleExportItem>>
            {
                new ExcelExportColumn<SampleExportItem, SampleStatus>("Status", null, x => x.Status),
            })
            {
                CreateHeaders = true
            };
            sheet.Columns[0].Features.Add(new ExcelExportValidationFeature(ExcelExportEnumHelper.GetDisplayNames(typeof(SampleStatus))));

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(
                _service, sheet, new[] { new SampleExportItem { Status = SampleStatus.Draft } });
            var validations = workbook.GetSheet("Sheet1").GetDataValidations();

            Assert.NotEmpty(validations);
            Assert.Contains("草稿", validations[0].ValidationConstraint.ExplicitListValues);
            Assert.Contains("已发布", validations[0].ValidationConstraint.ExplicitListValues);
        }

        [Fact]
        public void Context_Write_ProducesReadableWorkbook_AndThrowsAfterDispose()
        {
            var sheet = ExcelExportTestHelper.CreateBasicSheet();
            var items = new[] { new SampleExportItem { Name = "Alice", Age = 30, Score = 1 } };
            var context = (NpoiExcelExportContext)_service.CreateContext();
            _service.Export(context, sheet, items);

            using var ms = new MemoryStream();
            context.Write(ms);
            Assert.True(ms.Length > 0);

            ms.Position = 0;
            using (var workbook = new XSSFWorkbook(ms))
            {
                Assert.Equal("Alice", workbook.GetSheet("Sheet1").GetRow(1)!.GetCell(0)!.StringCellValue);
            }

            context.Dispose();
            Assert.Throws<ObjectDisposedException>(() => context.Write(new MemoryStream()));
        }

        [Fact]
        public void Export_ExpandingHeaders_MergeHeader_WritesParentAndChildRows()
        {
            var sheet = ExcelExportTestHelper.CreateOrderSheet(mergeHeader: true);
            var items = new[]
            {
                new SampleOrderExportItem
                {
                    Name = "Alice",
                    Age = 30,
                    Orders =
                    {
                        new SampleOrderLine { Id = 1, Qty = 2 },
                        new SampleOrderLine { Id = 3, Qty = 4 },
                    }
                }
            };

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(_service, sheet, items);
            var npoiSheet = workbook.GetSheet("Sheet1");

            Assert.Equal("Name", ExcelExportTestHelper.GetStringCell(npoiSheet, 0, 0));
            Assert.Equal("Orders", ExcelExportTestHelper.GetStringCell(npoiSheet, 0, 1));
            Assert.Equal("Age", ExcelExportTestHelper.GetStringCell(npoiSheet, 0, 3));
            Assert.Equal("Id", ExcelExportTestHelper.GetStringCell(npoiSheet, 1, 1));
            Assert.Equal("Qty", ExcelExportTestHelper.GetStringCell(npoiSheet, 1, 2));

            var merged = npoiSheet.MergedRegions;
            Assert.Contains(merged, r => r.FirstRow == 0 && r.LastRow == 0 && r.FirstColumn == 1 && r.LastColumn == 2);
            Assert.Contains(merged, r => r.FirstRow == 0 && r.LastRow == 1 && r.FirstColumn == 0 && r.LastColumn == 0);
            Assert.Contains(merged, r => r.FirstRow == 0 && r.LastRow == 1 && r.FirstColumn == 3 && r.LastColumn == 3);

            Assert.Equal("Alice", ExcelExportTestHelper.GetStringCell(npoiSheet, 2, 0));
            Assert.Equal(1, npoiSheet.GetRow(2)!.GetCell(1)!.NumericCellValue);
            Assert.Equal(2, npoiSheet.GetRow(2)!.GetCell(2)!.NumericCellValue);
            Assert.Equal(30, npoiSheet.GetRow(2)!.GetCell(3)!.NumericCellValue);
            Assert.Equal("Alice", ExcelExportTestHelper.GetStringCell(npoiSheet, 3, 0));
            Assert.Equal(3, npoiSheet.GetRow(3)!.GetCell(1)!.NumericCellValue);
            Assert.Equal(4, npoiSheet.GetRow(3)!.GetCell(2)!.NumericCellValue);
            Assert.Equal(30, npoiSheet.GetRow(3)!.GetCell(3)!.NumericCellValue);
        }

        [Fact]
        public void Export_ExpandingHeaders_WithoutMerge_SkipsParentHeader()
        {
            var sheet = ExcelExportTestHelper.CreateOrderSheet(mergeHeader: false);
            var items = new[]
            {
                new SampleOrderExportItem
                {
                    Name = "Alice",
                    Age = 30,
                    Orders =
                    {
                        new SampleOrderLine { Id = 1, Qty = 2 },
                        new SampleOrderLine { Id = 3, Qty = 4 },
                    }
                }
            };

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(_service, sheet, items);
            var npoiSheet = workbook.GetSheet("Sheet1");

            // Single header row: Name | Id | Qty | Age (no "Orders" parent header)
            Assert.Equal("Name", ExcelExportTestHelper.GetStringCell(npoiSheet, 0, 0));
            Assert.Equal("Id", ExcelExportTestHelper.GetStringCell(npoiSheet, 0, 1));
            Assert.Equal("Qty", ExcelExportTestHelper.GetStringCell(npoiSheet, 0, 2));
            Assert.Equal("Age", ExcelExportTestHelper.GetStringCell(npoiSheet, 0, 3));
            Assert.Empty(npoiSheet.MergedRegions);

            // First expanded order line
            Assert.Equal("Alice", ExcelExportTestHelper.GetStringCell(npoiSheet, 1, 0));
            Assert.Equal(1, npoiSheet.GetRow(1)!.GetCell(1)!.NumericCellValue);
            Assert.Equal(2, npoiSheet.GetRow(1)!.GetCell(2)!.NumericCellValue);
            Assert.Equal(30, npoiSheet.GetRow(1)!.GetCell(3)!.NumericCellValue);

            // Second expanded order line (parent Name/Age copied; Age column index still 3)
            Assert.Equal("Alice", ExcelExportTestHelper.GetStringCell(npoiSheet, 2, 0));
            Assert.Equal(3, npoiSheet.GetRow(2)!.GetCell(1)!.NumericCellValue);
            Assert.Equal(4, npoiSheet.GetRow(2)!.GetCell(2)!.NumericCellValue);
            Assert.Equal(30, npoiSheet.GetRow(2)!.GetCell(3)!.NumericCellValue);
        }

        private static string GetColumnDataFormat(ISheet sheet, int column)
        {
            var columnStyle = sheet.GetColumnStyle(column);
            var columnFormat = columnStyle?.GetDataFormatString();
            if (!string.IsNullOrEmpty(columnFormat) && columnFormat != "General")
                return columnFormat;

            var cell = sheet.GetRow(sheet.FirstRowNum + 1)?.GetCell(column);
            return cell?.CellStyle?.GetDataFormatString() ?? "General";
        }

        private sealed class FakeExcelExportContext : IExcelExportContext
        {
            public IServiceProvider? Services => null;

            public void Dispose() { }

            public void Write(Stream stream) { }

            public Task WriteAsync(Stream stream) => Task.CompletedTask;
        }
    }
}