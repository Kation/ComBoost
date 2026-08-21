namespace Wodsoft.ComBoost.ExcelExport
{
    /// <summary>
    /// Marks a collection property as expandable so that nested items are written as additional columns or rows.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExcelExportExpandableAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelExportExpandableAttribute"/> class.
        /// The item type is inferred from <c>IEnumerable&lt;T&gt;</c> on the property.
        /// </summary>
        public ExcelExportExpandableAttribute() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelExportExpandableAttribute"/> class.
        /// </summary>
        /// <param name="itemType">The type of each expanded item.</param>
        public ExcelExportExpandableAttribute(Type itemType)
        {
            ItemType = itemType;
        }

        /// <summary>
        /// Gets the type of each expanded item, or <see langword="null"/> to infer it from the property type.
        /// </summary>
        public Type? ItemType { get; }
    }
}
