using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.XSSF.Streaming;
using NPOI.XSSF.UserModel;
using NPOI.XSSF.UserModel.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.ExcelExport.NPOI
{
    /// <summary>
    /// An NPOI-backed export context that owns an <see cref="SXSSFWorkbook"/>.
    /// </summary>
    public class NpoiExcelExportContext : IExcelExportContext
    {
        private readonly SXSSFWorkbook _workbook;
        private readonly Dictionary<int, XSSFCellStyle> _styles;
        private readonly Dictionary<int, XSSFFont> _fonts;
        private readonly IServiceProvider? _services;
        private int _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="NpoiExcelExportContext"/> class.
        /// </summary>
        /// <param name="services">The service provider available to the context, or <see langword="null"/>.</param>
        /// <param name="workbook">The streaming workbook to write.</param>
        /// <param name="settings">The export settings.</param>
        public NpoiExcelExportContext(IServiceProvider? services, SXSSFWorkbook workbook, NpoiExcelExportSettings settings)
        {
            _services = services;
            _workbook = workbook;
            Settings = settings;
            _styles = new Dictionary<int, XSSFCellStyle>();
            _fonts = new Dictionary<int, XSSFFont>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpoiExcelExportContext"/> class with a new streaming workbook.
        /// </summary>
        /// <param name="services">The service provider available to the context, or <see langword="null"/>.</param>
        /// <param name="settings">The export settings.</param>
        public NpoiExcelExportContext(IServiceProvider? services, NpoiExcelExportSettings settings) : this(services, new SXSSFWorkbook(2000), settings) { }

        /// <inheritdoc />
        public IServiceProvider? Services => _services;

        /// <summary>
        /// Gets the underlying streaming workbook.
        /// </summary>
        public SXSSFWorkbook WorkBook => _workbook;

        /// <summary>
        /// Gets the export settings.
        /// </summary>
        public NpoiExcelExportSettings Settings { get; }

        /// <summary>
        /// Gets or creates a cached cell style for the specified RGB colors.
        /// </summary>
        /// <param name="background">The background RGB bytes, or an empty span.</param>
        /// <param name="foreground">The foreground RGB bytes, or an empty span.</param>
        /// <param name="style">An existing style to update, or <see langword="null"/> to use or create a cached style.</param>
        /// <returns>The cell style, or <see langword="null"/> if either color span is empty.</returns>
        public XSSFCellStyle? GetStyle(ReadOnlySpan<byte> background, ReadOnlySpan<byte> foreground, XSSFCellStyle? style = null)
        {
            if (background.Length == 0 || foreground.Length == 0)
                return null;
            int hash;
            if (background.Length != 0 && foreground.Length != 0)
            {
                if (background.Length != 3)
                    throw new ArgumentException("Background length must be 3.");
                if (foreground.Length != 3)
                    throw new ArgumentException("Foreground length must be 3.");
                hash = HashCode.Combine(background[0], background[1], background[2], foreground[0], foreground[1], foreground[2]);
            }
            else if (background.Length != 0)
            {
                if (background.Length != 3)
                    throw new ArgumentException("Background length must be 3.");
                hash = HashCode.Combine(background[0], background[1], background[2]);
            }
            else
            {
                if (foreground.Length != 3)
                    throw new ArgumentException("Foreground length must be 3.");
                hash = HashCode.Combine(foreground[0], foreground[1], foreground[2]);
            }
            if (style != null || !_styles.TryGetValue(hash, out style))
            {
                var stylesTable = WorkBook.XssfWorkbook.GetStylesSource();
                style ??= (XSSFCellStyle)WorkBook.CreateCellStyle();
                style.FillPattern = FillPattern.SolidForeground;
                _styles.Add(hash, style);
                if (background.Length != 0)
                {
                    CT_Color color = new CT_Color();
                    color.SetRgb(background[0], background[1], background[2]);
                    XSSFColor xssfColor = new XSSFColor(color, null);
                    var fill = new CT_Fill();
                    fill.AddNewPatternFill().fgColor = color;
                    int idx = stylesTable.PutFill(new XSSFCellFill(fill, stylesTable.IndexedColors));
                    var xf = style.GetCoreXf();
                    xf.fillId = (uint)idx;
                    xf.applyFill = true;
                }
                if (foreground.Length != 0)
                {
                    hash = HashCode.Combine(foreground[0], foreground[1], foreground[2]);
                    if (!_fonts.TryGetValue(hash, out var font))
                    {
                        font = (XSSFFont)WorkBook.CreateFont();
                        _fonts.Add(hash, font);
                        var color = font.GetCTFont().AddNewColor();
                        color.SetRgb(foreground[0], foreground[1], foreground[2]);
                    }
                    style.SetFont(font);
                }
            }
            return style;
        }

        /// <inheritdoc />
        public void Write(Stream stream)
        {
            if (_disposed != 0)
                throw new ObjectDisposedException(nameof(NpoiExcelExportContext));
            _workbook.Write(stream, true);
        }

        /// <inheritdoc />
        public Task WriteAsync(Stream stream)
        {
            if (_disposed != 0)
                throw new ObjectDisposedException(nameof(NpoiExcelExportContext));
            return Task.Run(() =>
            {
                _workbook.Write(stream, true);
            });
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 1)
                return;
            //_workbook.Close();
            _workbook.Dispose();
        }
    }
}
