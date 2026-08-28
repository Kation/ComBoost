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
            Assert.True(context.Settings.TryGetWriter(typeof(DateTimeOffset), out _));
#if NET6_0_OR_GREATER
            Assert.True(context.Settings.TryGetWriter(typeof(DateOnly), out _));
            Assert.True(context.Settings.TryGetWriter(typeof(TimeOnly), out _));
#endif
            Assert.True(context.Settings.TryGetWriter(typeof(SampleStatus), out _));
        }

        [Fact]
        public void Export_ThrowsWhenContextIsNotNpoi()
        {
            var sheet = ExcelExportTestHelper.CreateBasicSheet(_service);
            var fakeContext = new FakeExcelExportContext();

            var ex = Assert.Throws<ArgumentException>(() =>
                _service.Export(fakeContext, sheet, Array.Empty<SampleExportItem>()));

            Assert.Contains("NpoiExcelExportContext", ex.Message);
        }

        [Fact]
        public async Task ExportAsync_ThrowsWhenContextIsNotNpoi()
        {
            var sheet = ExcelExportTestHelper.CreateBasicSheet(_service);
            var fakeContext = new FakeExcelExportContext();

            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.ExportAsync(fakeContext, sheet, ExcelExportTestHelper.ToAsyncEnumerable(Array.Empty<SampleExportItem>())));

            Assert.Contains("NpoiExcelExportContext", ex.Message);
        }

        [Fact]
        public void Export_WithHeaders_WritesHeaderAndDataRows()
        {
            var sheet = ExcelExportTestHelper.CreateBasicSheet(_service, "People");
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
            var sheet = ExcelExportTestHelper.CreateBasicSheet(_service);
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
            var sheet = ExcelExportTestHelper.CreateBasicSheet(_service, null);
            var items = new[] { new SampleExportItem { Name = "Alice", Age = 1, Score = 1 } };

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(_service, sheet, items);

            Assert.Equal(1, workbook.NumberOfSheets);
            Assert.Equal("Alice", ExcelExportTestHelper.GetStringCell(workbook.GetSheetAt(0), 1, 0));
        }

        [Fact]
        public void Export_RespectsStartRowAndStartColumn()
        {
            var sheet = ExcelExportTestHelper.CreateBasicSheet(_service);
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
            var sheet = ExcelExportTestHelper.CreateBasicSheet(_service);

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(_service, sheet, Array.Empty<SampleExportItem>());
            var npoiSheet = workbook.GetSheet("Sheet1");

            Assert.Equal("Name", ExcelExportTestHelper.GetStringCell(npoiSheet, 0, 0));
            Assert.Null(npoiSheet.GetRow(1));
        }

        [Fact]
        public async Task ExportAsync_MatchesSyncResults()
        {
            var sheet = ExcelExportTestHelper.CreateBasicSheet(_service, "Async");
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
            var sheet = ExcelExportTestHelper.CreateTypedSheet(_service);
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
        public void Export_DateTimeAndDateTimeOffset_WritesExcelDateValues()
        {
            var createdAt = new DateTime(2024, 5, 6, 12, 30, 0, DateTimeKind.Unspecified);
            var occurredAt = new DateTimeOffset(2024, 5, 6, 20, 30, 0, TimeSpan.FromHours(8));
            var sheet = _service.GetExportSheet<SampleExportItem>();
            sheet.Name = "Sheet1";
            sheet.KeepClrColumns(nameof(SampleExportItem.CreatedAt), nameof(SampleExportItem.OccurredAt));
            var items = new[]
            {
                new SampleExportItem { CreatedAt = createdAt, OccurredAt = occurredAt }
            };

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(_service, sheet, items);
            var npoiSheet = workbook.GetSheet("Sheet1");
            var row = npoiSheet.GetRow(1)!;
            var dateTimeCell = row.GetCell(0)!;
            var dateTimeOffsetCell = row.GetCell(1)!;

            var stream = File.Open("test.xlsx", FileMode.Create);
            workbook.Write(stream);

            Assert.Equal("CreatedAt", ExcelExportTestHelper.GetStringCell(npoiSheet, 0, 0));
            Assert.Equal("OccurredAt", ExcelExportTestHelper.GetStringCell(npoiSheet, 0, 1));
            Assert.Equal(CellType.Numeric, dateTimeCell.CellType);
            Assert.Equal(createdAt, dateTimeCell.DateCellValue);
            Assert.Equal(CellType.Numeric, dateTimeOffsetCell.CellType);
            Assert.Equal(occurredAt.LocalDateTime, dateTimeOffsetCell.DateCellValue);
        }

        [Fact]
        public void Export_NullableDateTimeAndDateTimeOffset_WritesValuesAndRemovesNullCells()
        {
            var createdAt = new DateTime(2024, 5, 6, 12, 30, 0, DateTimeKind.Unspecified);
            var occurredAt = new DateTimeOffset(2024, 5, 6, 20, 30, 0, TimeSpan.FromHours(8));
            var sheet = _service.GetExportSheet<SampleExportItem>();
            sheet.Name = "Sheet1";
            sheet.KeepClrColumns(nameof(SampleExportItem.OptionalCreatedAt), nameof(SampleExportItem.OptionalOccurredAt));
            var items = new[]
            {
                new SampleExportItem { OptionalCreatedAt = createdAt, OptionalOccurredAt = occurredAt },
                new SampleExportItem { OptionalCreatedAt = null, OptionalOccurredAt = null },
            };

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(_service, sheet, items);
            var npoiSheet = workbook.GetSheet("Sheet1");
            var valueRow = npoiSheet.GetRow(1)!;
            var nullRow = npoiSheet.GetRow(2)!;

            Assert.Equal(createdAt, valueRow.GetCell(0)!.DateCellValue);
            Assert.Equal(occurredAt.LocalDateTime, valueRow.GetCell(1)!.DateCellValue);
            Assert.Null(nullRow.GetCell(0));
            Assert.Null(nullRow.GetCell(1));
        }

        [Fact]
        public void Export_DateTimeAndDateTimeOffset_AppliesExcelDataFormat()
        {
            var createdAt = new DateTime(2024, 5, 6, 12, 30, 0, DateTimeKind.Unspecified);
            var occurredAt = new DateTimeOffset(2024, 5, 6, 20, 30, 0, TimeSpan.FromHours(8));
            var sheet = _service.GetExportSheet<SampleExportItem>();
            sheet.Name = "Sheet1";
            sheet.KeepClrColumns(nameof(SampleExportItem.CreatedAt), nameof(SampleExportItem.OccurredAt));
            sheet.Columns[0].SetDataFormat("yyyy-mm-dd hh:mm:ss");
            sheet.Columns[1].SetDataFormat("yyyy-mm-dd hh:mm:ss");

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(_service, sheet, new[]
            {
                new SampleExportItem { CreatedAt = createdAt, OccurredAt = occurredAt }
            });
            var npoiSheet = workbook.GetSheet("Sheet1");

            Assert.Equal("yyyy-mm-dd hh:mm:ss", GetColumnDataFormat(npoiSheet, 0));
            Assert.Equal("yyyy-mm-dd hh:mm:ss", GetColumnDataFormat(npoiSheet, 1));
            Assert.Equal(createdAt, npoiSheet.GetRow(1)!.GetCell(0)!.DateCellValue);
            Assert.Equal(occurredAt.LocalDateTime, npoiSheet.GetRow(1)!.GetCell(1)!.DateCellValue);
        }

        [Fact]
        public void Export_GetExportSheet_WritesDateTimeAndDateTimeOffsetProperties()
        {
            var createdAt = new DateTime(2024, 5, 6, 12, 30, 0, DateTimeKind.Unspecified);
            var occurredAt = new DateTimeOffset(2024, 5, 6, 20, 30, 0, TimeSpan.FromHours(8));
            var sheet = _service.GetExportSheet<SampleDateTimeExportItem>();
            sheet.CreateHeaders = true;

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(_service, sheet, new[]
            {
                new SampleDateTimeExportItem
                {
                    Id = 1,
                    CreatedAt = createdAt,
                    OptionalCreatedAt = createdAt,
                    OccurredAt = occurredAt,
                    OptionalOccurredAt = occurredAt
                }
            });
            var npoiSheet = workbook.GetSheetAt(0);
            var createdAtIndex = sheet.Columns.ToList().FindIndex(t => t.ClrProperty?.Name == nameof(SampleDateTimeExportItem.CreatedAt));
            var occurredAtIndex = sheet.Columns.ToList().FindIndex(t => t.ClrProperty?.Name == nameof(SampleDateTimeExportItem.OccurredAt));
            var row = npoiSheet.GetRow(1)!;

            Assert.True(createdAtIndex >= 0);
            Assert.True(occurredAtIndex >= 0);
            Assert.Equal(typeof(DateTime), sheet.Columns[createdAtIndex].Type);
            Assert.Equal(typeof(DateTimeOffset), sheet.Columns[occurredAtIndex].Type);
            Assert.Equal(createdAt, row.GetCell(createdAtIndex)!.DateCellValue);
            Assert.Equal(occurredAt.LocalDateTime, row.GetCell(occurredAtIndex)!.DateCellValue);
        }

#if NET6_0_OR_GREATER
        [Fact]
        public void Export_DateOnlyAndTimeOnly_WritesExcelDateValues()
        {
            var date = new DateOnly(2024, 5, 6);
            var time = new TimeOnly(12, 30, 0);
            var sheet = _service.GetExportSheet<SampleExportItem>();
            sheet.Name = "Sheet1";
            sheet.KeepClrColumns(nameof(SampleExportItem.Date), nameof(SampleExportItem.Time));
            var items = new[]
            {
                new SampleExportItem { Date = date, Time = time }
            };

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(_service, sheet, items);
            var npoiSheet = workbook.GetSheet("Sheet1");
            var row = npoiSheet.GetRow(1)!;
            var dateCell = row.GetCell(0)!;
            var timeCell = row.GetCell(1)!;

            Assert.Equal("Date", ExcelExportTestHelper.GetStringCell(npoiSheet, 0, 0));
            Assert.Equal("Time", ExcelExportTestHelper.GetStringCell(npoiSheet, 0, 1));
            Assert.Equal(CellType.Numeric, dateCell.CellType);
            Assert.Equal(date.ToDateTime(TimeOnly.MinValue), dateCell.DateCellValue);
            Assert.Equal(CellType.Numeric, timeCell.CellType);
            Assert.Equal(time.ToTimeSpan().TotalDays, timeCell.NumericCellValue, 10);
            Assert.Equal(time.ToTimeSpan(), timeCell.DateCellValue!.Value.TimeOfDay);
        }

        [Fact]
        public void Export_NullableDateOnlyAndTimeOnly_WritesValuesAndRemovesNullCells()
        {
            var date = new DateOnly(2024, 5, 6);
            var time = new TimeOnly(12, 30, 0);
            var sheet = _service.GetExportSheet<SampleExportItem>();
            sheet.Name = "Sheet1";
            sheet.KeepClrColumns(nameof(SampleExportItem.OptionalDate), nameof(SampleExportItem.OptionalTime));
            var items = new[]
            {
                new SampleExportItem { OptionalDate = date, OptionalTime = time },
                new SampleExportItem { OptionalDate = null, OptionalTime = null },
            };

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(_service, sheet, items);
            var npoiSheet = workbook.GetSheet("Sheet1");
            var valueRow = npoiSheet.GetRow(1)!;
            var nullRow = npoiSheet.GetRow(2)!;

            Assert.Equal(date.ToDateTime(TimeOnly.MinValue), valueRow.GetCell(0)!.DateCellValue);
            Assert.Equal(time.ToTimeSpan(), valueRow.GetCell(1)!.DateCellValue!.Value.TimeOfDay);
            Assert.Null(nullRow.GetCell(0));
            Assert.Null(nullRow.GetCell(1));
        }

        [Fact]
        public void Export_DateOnlyAndTimeOnly_AppliesExcelDataFormat()
        {
            var date = new DateOnly(2024, 5, 6);
            var time = new TimeOnly(12, 30, 0);
            var sheet = _service.GetExportSheet<SampleExportItem>();
            sheet.Name = "Sheet1";
            sheet.KeepClrColumns(nameof(SampleExportItem.Date), nameof(SampleExportItem.Time));
            sheet.Columns[0].SetDataFormat("yyyy-mm-dd");
            sheet.Columns[1].SetDataFormat("hh:mm:ss");

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(_service, sheet, new[]
            {
                new SampleExportItem { Date = date, Time = time }
            });
            var npoiSheet = workbook.GetSheet("Sheet1");

            Assert.Equal("yyyy-mm-dd", GetColumnDataFormat(npoiSheet, 0));
            Assert.Equal("hh:mm:ss", GetColumnDataFormat(npoiSheet, 1));
            Assert.Equal(date.ToDateTime(TimeOnly.MinValue), npoiSheet.GetRow(1)!.GetCell(0)!.DateCellValue);
            Assert.Equal(time.ToTimeSpan(), npoiSheet.GetRow(1)!.GetCell(1)!.DateCellValue!.Value.TimeOfDay);
        }

        [Fact]
        public void Export_GetExportSheet_WritesDateOnlyAndTimeOnlyProperties()
        {
            var date = new DateOnly(2024, 5, 6);
            var time = new TimeOnly(12, 30, 0);
            var sheet = _service.GetExportSheet<SampleDateTimeExportItem>();
            sheet.CreateHeaders = true;

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(_service, sheet, new[]
            {
                new SampleDateTimeExportItem
                {
                    Id = 1,
                    Date = date,
                    OptionalDate = date,
                    Time = time,
                    OptionalTime = time
                }
            });
            var npoiSheet = workbook.GetSheetAt(0);
            var dateIndex = sheet.Columns.ToList().FindIndex(t => t.ClrProperty?.Name == nameof(SampleDateTimeExportItem.Date));
            var timeIndex = sheet.Columns.ToList().FindIndex(t => t.ClrProperty?.Name == nameof(SampleDateTimeExportItem.Time));
            var row = npoiSheet.GetRow(1)!;

            Assert.True(dateIndex >= 0);
            Assert.True(timeIndex >= 0);
            Assert.Equal(typeof(DateOnly), sheet.Columns[dateIndex].Type);
            Assert.Equal(typeof(TimeOnly), sheet.Columns[timeIndex].Type);
            Assert.Equal(date.ToDateTime(TimeOnly.MinValue), row.GetCell(dateIndex)!.DateCellValue);
            Assert.Equal(time.ToTimeSpan(), row.GetCell(timeIndex)!.DateCellValue!.Value.TimeOfDay);
        }
#endif

        [Fact]
        public void Export_NullValues_RemovesCellByDefault()
        {
            var sheet = ExcelExportTestHelper.CreateBasicSheet(_service);
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
            var sheet = ExcelExportTestHelper.CreateBasicSheet(_service);
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
            var sheet = _service.GetExportSheet<SampleExportItem>();
            sheet.Name = "Sheet1";
            sheet.KeepClrColumns(nameof(SampleExportItem.Name));
            sheet.OverrideColumn(x => x.Name, (SampleExportItem _) => Guid.NewGuid());

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
            var sheet = _service.GetExportSheet<SampleExportItem>();
            sheet.Name = "Sheet1";
            sheet.KeepClrColumns(nameof(SampleExportItem.Name), nameof(SampleExportItem.Age));
            sheet.GetClrColumn(x => x.Name).Width = 4000;
            var items = new[] { new SampleExportItem { Name = "Alice", Age = 30 } };

            using var workbook = ExcelExportTestHelper.ExportToWorkbook(_service, sheet, items);
            var npoiSheet = workbook.GetSheet("Sheet1");

            Assert.Equal(4000, npoiSheet.GetColumnWidth(0));
            Assert.True(npoiSheet.GetColumnWidth(1) > 0);
        }

        [Fact]
        public void Export_ColorFeature_AppliesHeaderStyle()
        {
            var sheet = ExcelExportTestHelper.CreateBasicSheet(_service);
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
            var sheet = _service.GetExportSheet<SampleExportItem>();
            sheet.Name = "Sheet1";
            sheet.KeepClrColumns(nameof(SampleExportItem.CreatedAt), nameof(SampleExportItem.DecimalValue));
            sheet.Columns[0].SetDataFormat("yyyy-mm-dd");
            sheet.Columns[1].SetDataFormat("0.00");

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
            var sheet = _service.GetExportSheet<SampleExportItem>();
            sheet.Name = "Sheet1";
            sheet.KeepClrColumns(nameof(SampleExportItem.Status));

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
            var sheet = ExcelExportTestHelper.CreateBasicSheet(_service);
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
            var sheet = ExcelExportTestHelper.CreateBasicSheet(_service);
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
            var sheet = _service.GetExportSheet<SampleExportItem>();
            sheet.Name = "Sheet1";
            sheet.KeepClrColumns(nameof(SampleExportItem.Status));

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
            var sheet = ExcelExportTestHelper.CreateBasicSheet(_service);
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
            var sheet = ExcelExportTestHelper.CreateOrderSheet(_service, mergeHeader: true);
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
            var sheet = ExcelExportTestHelper.CreateOrderSheet(_service, mergeHeader: false);
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