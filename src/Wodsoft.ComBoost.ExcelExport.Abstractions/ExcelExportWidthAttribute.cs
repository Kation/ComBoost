namespace Wodsoft.ComBoost.ExcelExport
{
    /// <summary>
    /// Specifies the Excel column width for an exported property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExcelExportWidthAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelExportWidthAttribute"/> class.
        /// </summary>
        /// <param name="width">The column width in Excel units.</param>
        public ExcelExportWidthAttribute(int width)
        {
            Width = width;
        }

        /// <summary>
        /// Gets the column width in Excel units.
        /// </summary>
        public int Width { get; }
    }
}
