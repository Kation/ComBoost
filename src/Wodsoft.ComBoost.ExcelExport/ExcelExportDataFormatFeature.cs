namespace Wodsoft.ComBoost.ExcelExport
{
    /// <summary>
    /// Applies an Excel data format to a column's data cells.
    /// </summary>
    public class ExcelExportDataFormatFeature : IExcelExportDataFormatFeature
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelExportDataFormatFeature"/> class.
        /// </summary>
        /// <param name="dataFormat">The Excel data format string.</param>
        public ExcelExportDataFormatFeature(string? dataFormat)
        {
            DataFormat = dataFormat;
        }

        /// <inheritdoc />
        public string? DataFormat { get; }
    }
}
