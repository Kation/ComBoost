namespace Wodsoft.ComBoost.ExcelExport
{
    /// <summary>
    /// Applies RGB colors to header and content cells of a column.
    /// </summary>
    public class ExcelExportColorFeature : IExcelExportColorFeature
    {
        private readonly byte[]? _headerBackground, _headerForeground, _contentBackground, _contentForeground;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelExportColorFeature"/> class.
        /// </summary>
        /// <param name="headerBackground">The header background RGB bytes, or <see langword="null"/>.</param>
        /// <param name="headerForeground">The header foreground RGB bytes, or <see langword="null"/>.</param>
        /// <param name="contentBackground">The content background RGB bytes, or <see langword="null"/>.</param>
        /// <param name="contentForeground">The content foreground RGB bytes, or <see langword="null"/>.</param>
        public ExcelExportColorFeature(byte[]? headerBackground, byte[]? headerForeground, byte[]? contentBackground, byte[]? contentForeground)
        {
            _headerBackground = headerBackground;
            _headerForeground = headerForeground;
            _contentBackground = contentBackground;
            _contentForeground = contentForeground;
        }

        /// <inheritdoc />
        public bool HasHeaderBackground => _headerBackground != null;

        /// <inheritdoc />
        public bool HasHeaderForeground => _headerForeground != null;

        /// <inheritdoc />
        public bool HasContentBackground => _contentBackground != null;

        /// <inheritdoc />
        public bool HasContentForeground => _contentForeground != null;

        /// <inheritdoc />
        public ReadOnlySpan<byte> HeaderBackground => _headerBackground ?? new ReadOnlySpan<byte>();

        /// <inheritdoc />
        public ReadOnlySpan<byte> HeaderForeground => _headerForeground ?? new ReadOnlySpan<byte>();

        /// <inheritdoc />
        public ReadOnlySpan<byte> ContentBackground => _contentBackground ?? new ReadOnlySpan<byte>();

        /// <inheritdoc />
        public ReadOnlySpan<byte> ContentForeground => _contentForeground ?? new ReadOnlySpan<byte>();
    }
}
