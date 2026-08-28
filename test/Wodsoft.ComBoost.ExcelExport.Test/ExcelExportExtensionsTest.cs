using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Wodsoft.ComBoost.ExcelExport.NPOI;
using Wodsoft.ComBoost.ExcelExport.Test.Helpers;
using Wodsoft.ComBoost.ExcelExport.Test.Models;

namespace Wodsoft.ComBoost.ExcelExport.Test
{
    public class ExcelExportExtensionsTest
    {
        private readonly NpoiExcelExportService _service = new();

        [Fact]
        public void GetClrColumn_ByPropertySelector_ReturnsMatchingColumn()
        {
            var sheet = CreateSheet(nameof(SampleExportItem.Name), nameof(SampleExportItem.Age));
            var nameColumn = sheet.GetClrColumn(x => x.Name);

            var actual = sheet.GetClrColumn(x => x.Name);

            Assert.Same(nameColumn, actual);
        }

        [Fact]
        public void GetClrColumn_ByPropertySelector_MatchesInheritedProperty()
        {
            var sheet = _service.GetExportSheet<DerivedSampleExportItem>();

            var actual = sheet.GetClrColumn(x => x.Name);

            Assert.Equal(nameof(SampleExportItem.Name), actual.ClrProperty!.Name);
            Assert.Equal(typeof(SampleExportItem), actual.ClrProperty.DeclaringType);
        }

        [Fact]
        public void GetClrColumn_ByPropertySelector_WhenDerivedModelSelectsBaseProperty_ReturnsColumn()
        {
            var sheet = _service.GetExportSheet<ChildExportModel>();

            var actual = sheet.GetClrColumn(x => x.Title);

            Assert.Equal(nameof(BaseExportModel.Title), actual.ClrProperty!.Name);
            Assert.Equal(typeof(BaseExportModel), actual.ClrProperty.DeclaringType);
            Assert.Same(actual, sheet.GetClrColumn(nameof(ChildExportModel.Title)));
        }

        [Fact]
        public void GetClrColumn_ByPropertySelector_WhenDerivedModelSelectsBasePropertyViaCast_ReturnsColumn()
        {
            var sheet = _service.GetExportSheet<ChildExportModel>();

            var actual = sheet.GetClrColumn(x => ((BaseExportModel)x).Title);

            Assert.Same(sheet.GetClrColumn(x => x.Title), actual);
        }

        [Fact]
        public void GetClrColumn_ByPropertySelector_ThrowsWhenExpressionIsNotProperty()
        {
            var sheet = CreateSheet(nameof(SampleExportItem.Name));

            var ex = Assert.Throws<InvalidOperationException>(() => sheet.GetClrColumn(x => x.Name + "!"));

            Assert.Contains("property", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void GetClrColumn_ByPropertySelector_ThrowsWhenColumnDoesNotExist()
        {
            var sheet = CreateSheet(nameof(SampleExportItem.Age));

            Assert.Throws<InvalidOperationException>(() => sheet.GetClrColumn(x => x.Name));
        }

        [Fact]
        public void GetClrColumn_ByName_ReturnsMatchingColumn()
        {
            var sheet = CreateSheet(nameof(SampleExportItem.Name), nameof(SampleExportItem.Age));
            var ageColumn = sheet.GetClrColumn(nameof(SampleExportItem.Age));

            var actual = sheet.GetClrColumn(nameof(SampleExportItem.Age));

            Assert.Same(ageColumn, actual);
        }

        [Fact]
        public void GetClrColumn_ByName_ThrowsWhenColumnDoesNotExist()
        {
            var sheet = CreateSheet(nameof(SampleExportItem.Name));

            var ex = Assert.Throws<InvalidOperationException>(() => sheet.GetClrColumn("Missing"));

            Assert.Contains("Missing", ex.Message);
        }

        [Fact]
        public void ReplaceColumn_ReplacesColumnAtSameIndex()
        {
            var sheet = CreateSheet(nameof(SampleExportItem.Name), nameof(SampleExportItem.Age));
            var nameColumn = sheet.GetClrColumn(x => x.Name);
            var ageColumn = sheet.GetClrColumn(x => x.Age);
            var replacement = nameColumn.Override(x => x.Name);

            var result = sheet.ReplaceColumn(nameColumn, replacement);

            Assert.Same(sheet, result);
            Assert.Same(replacement, sheet.Columns[0]);
            Assert.Same(ageColumn, sheet.Columns[1]);
            Assert.Equal(2, sheet.Columns.Count);
        }

        [Fact]
        public void ReplaceColumn_ThrowsWhenOldColumnIsNotOnSheet()
        {
            var sheet = CreateSheet(nameof(SampleExportItem.Name));
            var otherColumn = _service.GetExportColumn<SampleExportItem, int>(x => x.Age);
            var replacement = otherColumn.Override(x => x.Age);

            Assert.Throws<InvalidOperationException>(() => sheet.ReplaceColumn(otherColumn, replacement));
        }

        [Fact]
        public void OverrideColumn_WithValueReader_ReplacesColumnValue()
        {
            var sheet = CreateSheet(nameof(SampleExportItem.Name), nameof(SampleExportItem.Age));
            var nameColumn = sheet.GetClrColumn(x => x.Name);
            nameColumn.Width = 20;
            nameColumn.Features.Add(new ExcelExportCommentFeature("hint"));
            var item = new SampleExportItem { Name = "Alice" };

            var result = sheet.OverrideColumn(x => x.Name, x => x.Name + "!");

            Assert.Same(sheet, result);
            Assert.NotSame(nameColumn, sheet.Columns[0]);
            Assert.Equal("Name", sheet.Columns[0].Name);
            Assert.Equal(20, sheet.Columns[0].Width);
            Assert.Equal(Property(nameof(SampleExportItem.Name)), sheet.Columns[0].ClrProperty);
            Assert.True(sheet.Columns[0].TryGetFeature<IExcelExportCommentFeature>(out var comment));
            Assert.Equal("hint", comment!.HeaderComment);
            Assert.Equal(typeof(string), sheet.Columns[0].Type);

            var typed = Assert.IsAssignableFrom<IExcelExportColumn<SampleExportItem, string>>(sheet.Columns[0]);
            Assert.Equal("Alice!", typed.Read(item));
        }

        [Fact]
        public void OverrideColumn_WithValueConverter_ConvertsPropertyValue()
        {
            var sheet = CreateSheet(nameof(SampleExportItem.Name), nameof(SampleExportItem.Age));
            var item = new SampleExportItem { Name = "Alice" };

            sheet.OverrideColumn(x => x.Name, (string? name) => name?.ToUpperInvariant());

            var typed = Assert.IsAssignableFrom<IExcelExportColumn<SampleExportItem, string?>>(sheet.Columns[0]);
            Assert.Equal("ALICE", typed.Read(item));
            Assert.Same(sheet.Columns[1], sheet.GetClrColumn(x => x.Age));
        }

        [Fact]
        public void OverrideColumn_ThrowsWhenColumnDoesNotExist()
        {
            var sheet = CreateSheet(nameof(SampleExportItem.Age));

            Assert.Throws<InvalidOperationException>(() =>
                sheet.OverrideColumn(x => x.Name, x => x.Name));
        }

        private IExcelExportSheet<SampleExportItem> CreateSheet(params string[] clrNames)
        {
            var sheet = _service.GetExportSheet<SampleExportItem>();
            return sheet.KeepClrColumns(clrNames);
        }

        private static PropertyInfo Property(string name)
        {
            return typeof(SampleExportItem).GetProperty(name) ?? throw new InvalidOperationException(name);
        }

        private sealed class DerivedSampleExportItem : SampleExportItem
        {
        }

        private class BaseExportModel
        {
            [Key]
            public string? Title { get; set; }
        }

        private sealed class ChildExportModel : BaseExportModel
        {
            public int Count { get; set; }
        }
    }
}
