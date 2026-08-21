#if NET7_0_OR_GREATER
namespace Wodsoft.ComBoost.ExcelExport
{
    /// <summary>
    /// Provides a statically defined export sheet for <typeparamref name="TExport"/>.
    /// </summary>
    /// <typeparam name="TExport">The exported item type.</typeparam>
    public interface IExcelExportSheetMetadata<TExport>
    {
        /// <summary>
        /// Gets the predefined sheet definition.
        /// </summary>
        public static abstract IExcelExportSheet<TExport> Sheet { get; }
    }
}
#endif
