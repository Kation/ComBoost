namespace Wodsoft.ComBoost.ExcelExport
{
    /// <summary>
    /// Adds a comment to a column header cell.
    /// </summary>
    public class ExcelExportCommentFeature : IExcelExportCommentFeature
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelExportCommentFeature"/> class.
        /// </summary>
        /// <param name="headerComment">The header comment text.</param>
        public ExcelExportCommentFeature(string? headerComment)
        {
            HeaderComment = headerComment;
        }

        /// <inheritdoc />
        public string? HeaderComment { get; }
    }
}
