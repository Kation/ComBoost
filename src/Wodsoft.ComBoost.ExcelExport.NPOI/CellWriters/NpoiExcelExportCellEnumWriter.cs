using NPOI.SS.UserModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Wodsoft.ComBoost.ExcelExport.NPOI.CellWriters
{
    /// <summary>
    /// Writes enumeration values to NPOI cells as display names.
    /// </summary>
    public class NpoiExcelExportCellEnumWriter : INpoiExcelExportCellWriter
    {
        private readonly ConcurrentDictionary<Type, EnumFormatCache> _caches = new();

        /// <inheritdoc />
        public bool CanWrite(Type type) => type.IsEnum;

        /// <inheritdoc />
        public void Write<TExport>(NpoiExcelExportService service, NpoiExcelExportContext context, IExcelExportColumn<TExport> column, ICell cell, TExport item)
        {
            var enumType = Nullable.GetUnderlyingType(column.Type) ?? column.Type;
            if (!enumType.IsEnum)
                throw new InvalidOperationException("Invalid type of column.");

            var value = ReadColumnValue(column, item);
            if (value is null)
                return;

            var cache = _caches.GetOrAdd(enumType, static t => new EnumFormatCache(t));
            var formatted = cache.Format(value);
            if (formatted is not null)
                service.WriteCell(context, cell, column, item, formatted);
        }

        private static object? ReadColumnValue<TExport>(IExcelExportColumn<TExport> column, TExport item)
        {
            foreach (var iface in column.GetType().GetInterfaces())
            {
                if (!iface.IsGenericType || iface.GetGenericTypeDefinition() != typeof(IExcelExportColumn<,>))
                    continue;
                var args = iface.GetGenericArguments();
                if (args[0] != typeof(TExport))
                    continue;
                return iface.GetMethod(nameof(IExcelExportColumn<TExport, object>.Read), BindingFlags.Public | BindingFlags.Instance)!
                    .Invoke(column, new object?[] { item });
            }
            throw new InvalidOperationException($"Export column does not implement \"{typeof(IExcelExportColumn<TExport, object>).GetGenericTypeDefinition().FullName}\".");
        }

        private sealed class EnumFormatCache
        {
            private readonly bool _isFlags;
            private readonly Dictionary<ulong, string> _names;
            private readonly (ulong Value, string Name)[] _flagNames;

            public EnumFormatCache(Type enumType)
            {
                _isFlags = enumType.IsDefined(typeof(FlagsAttribute), false);
                _names = new Dictionary<ulong, string>();

                var fields = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);
                var flagNames = new List<(ulong Value, string Name)>(fields.Length);
                foreach (var field in fields)
                {
                    var displayName = ExcelExportEnumHelper.GetDisplayName(field);
                    var value = ExcelExportEnumHelper.ToUInt64(field.GetValue(null)!);
                    _names[value] = displayName;
                    if (value != 0 && ExcelExportEnumHelper.IsPowerOfTwo(value))
                        flagNames.Add((value, displayName));
                }

                _flagNames = flagNames
                    .OrderBy(t => t.Value)
                    .ToArray();
            }

            public string? Format(object value)
            {
                var bits = ExcelExportEnumHelper.ToUInt64(value);
                if (!_isFlags)
                {
                    if (_names.TryGetValue(bits, out var name))
                        return name;
                    return value.ToString();
                }

                if (bits == 0)
                {
                    if (_names.TryGetValue(0, out var zeroName))
                        return zeroName;
                    return value.ToString();
                }

                var parts = new List<string>();
                foreach (var (flagValue, flagName) in _flagNames)
                {
                    if ((bits & flagValue) == flagValue)
                        parts.Add(flagName);
                }

                if (parts.Count == 0)
                    return value.ToString();

                return string.Join(",", parts);
            }
        }
    }
}