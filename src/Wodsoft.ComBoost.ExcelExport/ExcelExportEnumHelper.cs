using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace Wodsoft.ComBoost.ExcelExport
{
    /// <summary>
    /// Provides helpers for reading display names and numeric values from enumeration types.
    /// </summary>
    public static class ExcelExportEnumHelper
    {
        /// <summary>
        /// Gets the display names of all public static fields on an enumeration type.
        /// </summary>
        /// <param name="enumType">The enumeration type.</param>
        /// <returns>The display names in field declaration order.</returns>
        public static string[] GetDisplayNames(Type enumType)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException($"Type \"{enumType.FullName}\" is not an enum type.", nameof(enumType));

            var fields = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);
            var displayNames = new string[fields.Length];
            for (int i = 0; i < fields.Length; i++)
                displayNames[i] = GetDisplayName(fields[i]);
            return displayNames;
        }

        /// <summary>
        /// Gets the display name of an enumeration field from <see cref="DisplayAttribute"/> or <see cref="DescriptionAttribute"/>.
        /// </summary>
        /// <param name="field">The enumeration field.</param>
        /// <returns>The display name, or the field name if no attribute is present.</returns>
        public static string GetDisplayName(FieldInfo field)
        {
            var display = field.GetCustomAttribute<DisplayAttribute>();
            if (display?.Name != null)
                return display.Name;

            var description = field.GetCustomAttribute<DescriptionAttribute>();
            if (description != null)
                return description.Description;

            return field.Name;
        }

        /// <summary>
        /// Converts an enumeration value to an unsigned 64-bit integer.
        /// </summary>
        /// <param name="value">The enumeration value.</param>
        /// <returns>The unsigned 64-bit representation of <paramref name="value"/>.</returns>
        public static ulong ToUInt64(object value)
        {
            return Convert.GetTypeCode(value) switch
            {
                TypeCode.SByte or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64
                    => unchecked((ulong)Convert.ToInt64(value, CultureInfo.InvariantCulture)),
                _
                    => Convert.ToUInt64(value, CultureInfo.InvariantCulture)
            };
        }

        /// <summary>
        /// Determines whether the specified value is a power of two.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns><see langword="true"/> if <paramref name="value"/> is a power of two; otherwise, <see langword="false"/>.</returns>
        public static bool IsPowerOfTwo(ulong value) => (value & (value - 1)) == 0;
    }
}
