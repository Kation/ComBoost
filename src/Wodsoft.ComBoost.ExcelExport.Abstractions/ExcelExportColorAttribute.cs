using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.ExcelExport
{
    /// <summary>
    /// Specifies header and content RGB colors for an exported property.
    /// Color strings use a comma-separated <c>R,G,B</c> format.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExcelExportColorAttribute : Attribute
    {
        private byte[]? _headerBg, _headerFg, _contentBg, _contentFg;

        /// <summary>
        /// Gets or sets the header background color as <c>R,G,B</c>.
        /// </summary>
        public string? HeaderBackGround
        {
#if NETSTANDARD2_0
            get => _headerBg == null ? null : string.Join(",", _headerBg); set
#else
            get => _headerBg == null ? null : string.Join(',', _headerBg); set
#endif
            {
                if (value == null)
                {
                    _headerBg = null;
                    return;
                }
#if NETSTANDARD2_0
                var values = value.Split([','], StringSplitOptions.RemoveEmptyEntries);
#else
                var values = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
#endif
                if (values.Length != 3)
                    throw new ArgumentException("值必须是RGB颜色以逗号分割。");
                if (!byte.TryParse(values[0], out var r))
                    throw new ArgumentException("值必须是RGB颜色以逗号分割。");
                if (!byte.TryParse(values[1], out var g))
                    throw new ArgumentException("值必须是RGB颜色以逗号分割。");
                if (!byte.TryParse(values[2], out var b))
                    throw new ArgumentException("值必须是RGB颜色以逗号分割。");
                _headerBg = new byte[3];
                _headerBg[0] = r;
                _headerBg[1] = g;
                _headerBg[2] = b;
            }
        }

        /// <summary>
        /// Gets or sets the header foreground color as <c>R,G,B</c>.
        /// </summary>
        public string? HeaderFrontGround
        {
#if NETSTANDARD2_0
            get => _headerBg == null ? null : string.Join(",", _headerFg); set
#else
            get => _headerBg == null ? null : string.Join(',', _headerFg); set
#endif
            {
                if (value == null)
                {
                    _headerFg = null;
                    return;
                }
#if NETSTANDARD2_0
                var values = value.Split([','], StringSplitOptions.RemoveEmptyEntries);
#else
                var values = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
#endif
                if (values.Length != 3)
                    throw new ArgumentException("值必须是RGB颜色以逗号分割。");
                if (!byte.TryParse(values[0], out var r))
                    throw new ArgumentException("值必须是RGB颜色以逗号分割。");
                if (!byte.TryParse(values[1], out var g))
                    throw new ArgumentException("值必须是RGB颜色以逗号分割。");
                if (!byte.TryParse(values[2], out var b))
                    throw new ArgumentException("值必须是RGB颜色以逗号分割。");
                _headerFg = new byte[3];
                _headerFg[0] = r;
                _headerFg[1] = g;
                _headerFg[2] = b;
            }
        }

        /// <summary>
        /// Gets or sets the content background color as <c>R,G,B</c>.
        /// </summary>
        public string? ContentBackGround
        {
#if NETSTANDARD2_0
            get => _headerBg == null ? null : string.Join(",", _contentBg); set
#else
            get => _headerBg == null ? null : string.Join(',', _contentBg); set
#endif
            {
                if (value == null)
                {
                    _contentBg = null;
                    return;
                }
#if NETSTANDARD2_0
                var values = value.Split([','], StringSplitOptions.RemoveEmptyEntries);
#else
                var values = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
#endif
                if (values.Length != 3)
                    throw new ArgumentException("值必须是RGB颜色以逗号分割。");
                if (!byte.TryParse(values[0], out var r))
                    throw new ArgumentException("值必须是RGB颜色以逗号分割。");
                if (!byte.TryParse(values[1], out var g))
                    throw new ArgumentException("值必须是RGB颜色以逗号分割。");
                if (!byte.TryParse(values[2], out var b))
                    throw new ArgumentException("值必须是RGB颜色以逗号分割。");
                _contentBg = new byte[3];
                _contentBg[0] = r;
                _contentBg[1] = g;
                _contentBg[2] = b;
            }
        }

        /// <summary>
        /// Gets or sets the content foreground color as <c>R,G,B</c>.
        /// </summary>
        public string? ContentFrontGround
        {
#if NETSTANDARD2_0
            get => _headerBg == null ? null : string.Join(",", _contentFg); set
#else
            get => _headerBg == null ? null : string.Join(',', _contentFg); set
#endif
            {
                if (value == null)
                {
                    _contentFg = null;
                    return;
                }
#if NETSTANDARD2_0
                var values = value.Split([','], StringSplitOptions.RemoveEmptyEntries);
#else
                var values = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
#endif
                if (values.Length != 3)
                    throw new ArgumentException("值必须是RGB颜色以逗号分割。");
                if (!byte.TryParse(values[0], out var r))
                    throw new ArgumentException("值必须是RGB颜色以逗号分割。");
                if (!byte.TryParse(values[1], out var g))
                    throw new ArgumentException("值必须是RGB颜色以逗号分割。");
                if (!byte.TryParse(values[2], out var b))
                    throw new ArgumentException("值必须是RGB颜色以逗号分割。");
                _contentFg = new byte[3];
                _contentFg[0] = r;
                _contentFg[1] = g;
                _contentFg[2] = b;
            }
        }

        /// <summary>
        /// Gets the parsed header background RGB bytes.
        /// </summary>
        /// <returns>A 3-byte RGB array, or <see langword="null"/> if not set.</returns>
        public byte[]? GetHeaderBackGround() => _headerBg;

        /// <summary>
        /// Gets the parsed header foreground RGB bytes.
        /// </summary>
        /// <returns>A 3-byte RGB array, or <see langword="null"/> if not set.</returns>
        public byte[]? GetHeaderFrontGround() => _headerFg;

        /// <summary>
        /// Gets the parsed content background RGB bytes.
        /// </summary>
        /// <returns>A 3-byte RGB array, or <see langword="null"/> if not set.</returns>
        public byte[]? GetContentBackGround() => _contentBg;

        /// <summary>
        /// Gets the parsed content foreground RGB bytes.
        /// </summary>
        /// <returns>A 3-byte RGB array, or <see langword="null"/> if not set.</returns>
        public byte[]? GetContentFrontGround() => _contentFg;
    }
}
