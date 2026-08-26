using System.Reflection;
using Wodsoft.ComBoost.ExcelExport.Test.Models;

namespace Wodsoft.ComBoost.ExcelExport.Test
{
    public class ExcelExportExtensionsTest
    {
        [Fact]
        public void GetClrColumn_ByPropertySelector_ReturnsMatchingColumn()
        {
            var nameColumn = CreateNameColumn();
            var sheet = CreateSheet(nameColumn, CreateAgeColumn());

            var actual = sheet.GetClrColumn(x => x.Name);

            Assert.Same(nameColumn, actual);
        }

        [Fact]
        public void GetClrColumn_ByPropertySelector_MatchesInheritedProperty()
        {
            var nameProperty = typeof(SampleExportItem).GetProperty(nameof(SampleExportItem.Name));
            var nameColumn = new ExcelExportColumn<DerivedSampleExportItem, string?>("Name", nameProperty, x => x.Name);
            var sheet = new ExcelExportSheet<DerivedSampleExportItem>(new List<IExcelExportColumn<DerivedSampleExportItem>>
            {
                nameColumn
            });

            var actual = sheet.GetClrColumn(x => x.Name);

            Assert.Same(nameColumn, actual);
        }

        [Fact]
        public void GetClrColumn_ByPropertySelector_WhenDerivedModelSelectsBaseProperty_ReturnsColumn()
        {
            var titleProperty = typeof(ChildExportModel).GetProperty(nameof(BaseExportModel.Title));
            var titleColumn = new ExcelExportColumn<ChildExportModel, string?>("Title", titleProperty, x => x.Title);
            var countColumn = new ExcelExportColumn<ChildExportModel, int>("Count", typeof(ChildExportModel).GetProperty(nameof(ChildExportModel.Count)), x => x.Count);
            var sheet = new ExcelExportSheet<ChildExportModel>(new List<IExcelExportColumn<ChildExportModel>>
            {
                titleColumn,
                countColumn
            });

            var actual = sheet.GetClrColumn(x => x.Title);

            Assert.Same(titleColumn, actual);
            Assert.Equal(typeof(BaseExportModel), actual.ClrProperty!.DeclaringType);
        }

        [Fact]
        public void GetClrColumn_ByPropertySelector_WhenDerivedModelSelectsBasePropertyViaCast_ReturnsColumn()
        {
            var titleProperty = typeof(ChildExportModel).GetProperty(nameof(BaseExportModel.Title));
            var titleColumn = new ExcelExportColumn<ChildExportModel, string?>("Title", titleProperty, x => x.Title);
            var sheet = new ExcelExportSheet<ChildExportModel>(new List<IExcelExportColumn<ChildExportModel>>
            {
                titleColumn
            });

            var actual = sheet.GetClrColumn(x => ((BaseExportModel)x).Title);

            Assert.Same(titleColumn, actual);
        }

        [Fact]
        public void GetClrColumn_ByPropertySelector_ThrowsWhenExpressionIsNotProperty()
        {
            var sheet = CreateSheet(CreateNameColumn());

            var ex = Assert.Throws<InvalidOperationException>(() => sheet.GetClrColumn(x => x.Name + "!"));

            Assert.Contains("property", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void GetClrColumn_ByPropertySelector_ThrowsWhenColumnDoesNotExist()
        {
            var sheet = CreateSheet(CreateAgeColumn());

            Assert.Throws<InvalidOperationException>(() => sheet.GetClrColumn(x => x.Name));
        }

        [Fact]
        public void GetClrColumn_ByName_ReturnsMatchingColumn()
        {
            var ageColumn = CreateAgeColumn();
            var sheet = CreateSheet(CreateNameColumn(), ageColumn);

            var actual = sheet.GetClrColumn(nameof(SampleExportItem.Age));

            Assert.Same(ageColumn, actual);
        }

        [Fact]
        public void GetClrColumn_ByName_ThrowsWhenColumnDoesNotExist()
        {
            var sheet = CreateSheet(CreateNameColumn());

            var ex = Assert.Throws<InvalidOperationException>(() => sheet.GetClrColumn("Missing"));

            Assert.Contains("Missing", ex.Message);
        }

        [Fact]
        public void ReplaceColumn_ReplacesColumnAtSameIndex()
        {
            var nameColumn = CreateNameColumn();
            var ageColumn = CreateAgeColumn();
            var replacement = new ExcelExportColumn<SampleExportItem, string?>("FullName", Property(nameof(SampleExportItem.Name)), x => x.Name);
            var sheet = CreateSheet(nameColumn, ageColumn);

            var result = sheet.ReplaceColumn(nameColumn, replacement);

            Assert.Same(sheet, result);
            Assert.Same(replacement, sheet.Columns[0]);
            Assert.Same(ageColumn, sheet.Columns[1]);
            Assert.Equal(2, sheet.Columns.Count);
        }

        [Fact]
        public void ReplaceColumn_ThrowsWhenOldColumnIsNotOnSheet()
        {
            var sheet = CreateSheet(CreateNameColumn());
            var otherColumn = CreateAgeColumn();
            var replacement = CreateAgeColumn();

            Assert.Throws<InvalidOperationException>(() => sheet.ReplaceColumn(otherColumn, replacement));
        }

        [Fact]
        public void OverrideColumn_WithValueReader_ReplacesColumnValue()
        {
            var nameColumn = CreateNameColumn();
            nameColumn.Width = 20;
            nameColumn.Features.Add(new ExcelExportCommentFeature("hint"));
            var sheet = CreateSheet(nameColumn, CreateAgeColumn());
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
            var sheet = CreateSheet(CreateNameColumn(), CreateAgeColumn());
            var item = new SampleExportItem { Name = "Alice" };

            sheet.OverrideColumn(x => x.Name, (string? name) => name?.ToUpperInvariant());

            var typed = Assert.IsAssignableFrom<IExcelExportColumn<SampleExportItem, string?>>(sheet.Columns[0]);
            Assert.Equal("ALICE", typed.Read(item));
            Assert.Same(sheet.Columns[1], sheet.GetClrColumn(x => x.Age));
        }

        [Fact]
        public void OverrideColumn_ThrowsWhenColumnDoesNotExist()
        {
            var sheet = CreateSheet(CreateAgeColumn());

            Assert.Throws<InvalidOperationException>(() =>
                sheet.OverrideColumn(x => x.Name, x => x.Name));
        }

        private static ExcelExportSheet<SampleExportItem> CreateSheet(params IExcelExportColumn<SampleExportItem>[] columns)
        {
            return new ExcelExportSheet<SampleExportItem>(new List<IExcelExportColumn<SampleExportItem>>(columns));
        }

        private static ExcelExportColumn<SampleExportItem, string?> CreateNameColumn()
        {
            return new ExcelExportColumn<SampleExportItem, string?>("Name", Property(nameof(SampleExportItem.Name)), x => x.Name);
        }

        private static ExcelExportColumn<SampleExportItem, int> CreateAgeColumn()
        {
            return new ExcelExportColumn<SampleExportItem, int>("Age", Property(nameof(SampleExportItem.Age)), x => x.Age);
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
            public string? Title { get; set; }
        }

        private sealed class ChildExportModel : BaseExportModel
        {
            public int Count { get; set; }
        }
    }
}
