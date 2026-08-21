namespace Wodsoft.ComBoost.ExcelExport
{
    /// <summary>
    /// Provides a comment attached to a column header cell.
    /// </summary>
    public interface IExcelExportCommentFeature : IExcelExportColumnFeature
    {
        /// <summary>
        /// Gets the header comment text, or <see langword="null"/> if none is set.
        /// </summary>
        string? HeaderComment { get; }
    }
}
