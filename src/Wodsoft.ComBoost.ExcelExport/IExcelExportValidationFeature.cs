namespace Wodsoft.ComBoost.ExcelExport
{
    /// <summary>
    /// Provides an explicit list used as Excel data validation for a column.
    /// </summary>
    public interface IExcelExportValidationFeature : IExcelExportColumnFeature
    {
        /// <summary>
        /// Gets the allowed values shown in the validation list.
        /// </summary>
        IReadOnlyList<string> Validations { get; }
    }
}
