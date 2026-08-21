using System.Globalization;
using Wodsoft.ComBoost.ExcelExport.NPOI;

namespace Wodsoft.ComBoost.ExcelExport.Test
{
    public class ExcelExportServiceFormatTest
    {
        private readonly TestExcelExportService _service = new();

        [Theory]
        [InlineData("{0:yyyy-MM-dd}", "yyyy-mm-dd")]
        [InlineData("yyyy-MM-dd", "yyyy-mm-dd")]
        [InlineData("{0:HH:mm:ss}", "hh:mm:ss")]
        [InlineData("{0:yyyy-MM-dd HH:mm:ss}", "yyyy-mm-dd hh:mm:ss")]
        [InlineData("{0:MMM}", "mmm")]
        [InlineData("{0:MMMM}", "mmmm")]
        [InlineData("{0:tt}", "AM/PM")]
        [InlineData("yyyy年MM月dd日", "yyyy年mm月dd日")]
        [InlineData("0.00", "0.00")]
        [InlineData("#,##0.00", "#,##0.00")]
        [InlineData("{0:N2}", "#,##0.00")]
        [InlineData("{0:N0}", "#,##0")]
        [InlineData("{0:F2}", "0.00")]
        [InlineData("{0:F0}", "0")]
        [InlineData("{0:P1}", "0.0%")]
        [InlineData("{0:D5}", "00000")]
        [InlineData("{0:E2}", "0.00E+00")]
        [InlineData("{0}", "General")]
        [InlineData("{0:G2}", "General")]
        [InlineData("{0:X}", "General")]
        public void ConvertToExcelFormat_MapsDotNetFormat(string format, string expected)
        {
            Assert.Equal(expected, _service.ToExcelFormat(format));
        }

        [Fact]
        public void ConvertToExcelFormat_MapsCurrencyUsingCurrentCulture()
        {
            var original = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("zh-CN");
                var actual = _service.ToExcelFormat("{0:C2}");
                Assert.Equal("\"¥\"#,##0.00", actual);
            }
            finally
            {
                CultureInfo.CurrentCulture = original;
            }
        }

        [Fact]
        public void ConvertToExcelFormat_MapsShortDateUsingCurrentCulture()
        {
            var original = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
                var actual = _service.ToExcelFormat("{0:d}");
                Assert.Equal("m/d/yyyy", actual);
            }
            finally
            {
                CultureInfo.CurrentCulture = original;
            }
        }

        private sealed class TestExcelExportService : NpoiExcelExportService
        {
            public string ToExcelFormat(string format) => ConvertToExcelFormat(format);
        }
    }
}