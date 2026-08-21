namespace Wodsoft.ComBoost.ExcelExport
{
    /// <summary>
    /// Exports typed data to Excel workbooks.
    /// </summary>
    public interface IExcelExportService
    {
        /// <summary>
        /// Creates a new export context that holds the workbook being written.
        /// </summary>
        /// <returns>An <see cref="IExcelExportContext"/> used by subsequent export calls.</returns>
        IExcelExportContext CreateContext();

        /// <summary>
        /// Creates a new export context that holds the workbook being written.
        /// </summary>
        /// <param name="services">The service provider available to the context.</param>
        /// <returns>An <see cref="IExcelExportContext"/> used by subsequent export calls.</returns>
        IExcelExportContext CreateContext(IServiceProvider? services);

        /// <summary>
        /// Exports an asynchronous sequence of items to the specified sheet.
        /// </summary>
        /// <typeparam name="TExport">The exported item type.</typeparam>
        /// <param name="context">The export context created by <see cref="CreateContext()"/>.</param>
        /// <param name="sheet">The sheet definition that describes columns and layout.</param>
        /// <param name="source">The items to export.</param>
        /// <returns>A task that completes when all items have been written.</returns>
        Task ExportAsync<TExport>(IExcelExportContext context, IExcelExportSheet<TExport> sheet, IAsyncEnumerable<TExport> source);

        /// <summary>
        /// Exports a sequence of items to the specified sheet.
        /// </summary>
        /// <typeparam name="TExport">The exported item type.</typeparam>
        /// <param name="context">The export context created by <see cref="CreateContext()"/>.</param>
        /// <param name="sheet">The sheet definition that describes columns and layout.</param>
        /// <param name="source">The items to export.</param>
        void Export<TExport>(IExcelExportContext context, IExcelExportSheet<TExport> sheet, IEnumerable<TExport> source);

        /// <summary>
        /// Builds a default sheet definition for <typeparamref name="TExport"/> from metadata or a static sheet descriptor.
        /// </summary>
        /// <typeparam name="TExport">The exported item type.</typeparam>
        /// <returns>The generated sheet definition.</returns>
        IExcelExportSheet<TExport> GetExportSheet<TExport>();
    }
}
