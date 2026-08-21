namespace Wodsoft.ComBoost.ExcelExport
{
    /// <summary>
    /// Represents a workbook that is being exported and can be written to a stream.
    /// </summary>
    public interface IExcelExportContext : IDisposable
    {
        /// <summary>
        /// Gets the service provider associated with this context, or <see langword="null"/> if none was provided.
        /// </summary>
        IServiceProvider? Services { get; }

        /// <summary>
        /// Writes the generated workbook to the specified stream.
        /// </summary>
        /// <param name="stream">The destination stream.</param>
        void Write(Stream stream);

        /// <summary>
        /// Writes the generated workbook to the specified stream asynchronously.
        /// </summary>
        /// <param name="stream">The destination stream.</param>
        /// <returns>A task that completes when the workbook has been written.</returns>
        Task WriteAsync(Stream stream);
    }
}
