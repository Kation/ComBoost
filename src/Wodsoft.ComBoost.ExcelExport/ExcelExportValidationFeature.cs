namespace Wodsoft.ComBoost.ExcelExport
{
    /// <summary>
    /// Adds an explicit Excel data-validation list to a column.
    /// </summary>
    public class ExcelExportValidationFeature : IExcelExportValidationFeature
    {
        private readonly string[] _values;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelExportValidationFeature"/> class.
        /// </summary>
        /// <param name="values">The allowed values.</param>
        public ExcelExportValidationFeature(string[] values)
        {
            _values = values;
        }

        /// <inheritdoc />
        public IReadOnlyList<string> Validations => _values;
    }
}
