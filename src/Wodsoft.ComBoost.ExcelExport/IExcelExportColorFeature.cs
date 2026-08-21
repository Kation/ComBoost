namespace Wodsoft.ComBoost.ExcelExport
{
    /// <summary>
    /// Provides RGB colors for header and content cells of a column.
    /// </summary>
    public interface IExcelExportColorFeature : IExcelExportColumnFeature
    {
        /// <summary>
        /// Gets a value indicating whether a header background color is set.
        /// </summary>
        bool HasHeaderBackground { get; }

        /// <summary>
        /// Gets a value indicating whether a header foreground color is set.
        /// </summary>
        bool HasHeaderForeground { get; }

        /// <summary>
        /// Gets a value indicating whether a content background color is set.
        /// </summary>
        bool HasContentBackground { get; }

        /// <summary>
        /// Gets a value indicating whether a content foreground color is set.
        /// </summary>
        bool HasContentForeground { get; }

        /// <summary>
        /// Gets the header background RGB bytes.
        /// </summary>
        ReadOnlySpan<byte> HeaderBackground { get; }

        /// <summary>
        /// Gets the header foreground RGB bytes.
        /// </summary>
        ReadOnlySpan<byte> HeaderForeground { get; }

        /// <summary>
        /// Gets the content background RGB bytes.
        /// </summary>
        ReadOnlySpan<byte> ContentBackground { get; }

        /// <summary>
        /// Gets the content foreground RGB bytes.
        /// </summary>
        ReadOnlySpan<byte> ContentForeground { get; }
    }
}
