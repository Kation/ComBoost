namespace Wodsoft.ComBoost.ExcelExport
{
    /// <summary>
    /// Provides an Excel number or date format applied to a column's data cells.
    /// </summary>
    public interface IExcelExportDataFormatFeature : IExcelExportColumnFeature
    {
        /// <summary>
        /// Gets the Excel data format string, or <see langword="null"/> if none is set.
        /// </summary>
        string? DataFormat { get; }
    }
}
