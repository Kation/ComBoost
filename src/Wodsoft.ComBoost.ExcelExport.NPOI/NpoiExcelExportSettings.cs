using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Wodsoft.ComBoost.ExcelExport.NPOI.CellWriters;

namespace Wodsoft.ComBoost.ExcelExport.NPOI
{
    /// <summary>
    /// Configures cell writers and null-value behavior for NPOI export.
    /// </summary>
    public class NpoiExcelExportSettings
    {
        private List<INpoiExcelExportCellWriter> _writers;

        /// <summary>
        /// Initializes a new instance of the <see cref="NpoiExcelExportSettings"/> class
        /// and registers the built-in cell writers.
        /// </summary>
        public NpoiExcelExportSettings()
        {
            _writers = new List<INpoiExcelExportCellWriter>();
            AddWriter(new NpoiExcelExportCellStringWriter());
            AddWriter(new NpoiExcelExportCellBooleanWriter());
            AddWriter(new NpoiExcelExportCellCharWriter());
            AddWriter(new NpoiExcelExportCellSByteWriter());
            AddWriter(new NpoiExcelExportCellByteWriter());
            AddWriter(new NpoiExcelExportCellInt16Writer());
            AddWriter(new NpoiExcelExportCellUInt16Writer());
            AddWriter(new NpoiExcelExportCellInt32Writer());
            AddWriter(new NpoiExcelExportCellUInt32Writer());
            AddWriter(new NpoiExcelExportCellInt64Writer());
            AddWriter(new NpoiExcelExportCellUInt64Writer());
            AddWriter(new NpoiExcelExportCellSingleWriter());
            AddWriter(new NpoiExcelExportCellDoubleWriter());
            AddWriter(new NpoiExcelExportCellDecimalWriter());
            AddWriter(new NpoiExcelExportCellDateTimeWriter());
            AddWriter(new NpoiExcelExportCellDateTimeOffsetWriter());
#if NET6_0_OR_GREATER
            AddWriter(new NpoiExcelExportCellDateOnlyWriter());
            AddWriter(new NpoiExcelExportCellTimeOnlyWriter());
#endif
            AddWriter(new NpoiExcelExportCellEnumWriter());
        }

        /// <summary>
        /// Registers a cell writer.
        /// </summary>
        /// <param name="writer">The writer to add.</param>
        public void AddWriter(INpoiExcelExportCellWriter writer)
        {
#if NET7_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(writer);
#else
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
#endif
            _writers.Add(writer);
        }

        /// <summary>
        /// Unregisters a cell writer.
        /// </summary>
        /// <param name="writer">The writer to remove.</param>
        public void RemoveWriter(INpoiExcelExportCellWriter writer)
        {
#if NET7_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(writer);
#else
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
#endif
            _writers.Remove(writer);
        }

        /// <summary>
        /// Tries to get a writer that can write values of the specified type.
        /// </summary>
        /// <param name="type">The CLR type.</param>
        /// <param name="writer">When this method returns, the matching writer, or <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if a writer was found; otherwise, <see langword="false"/>.</returns>
#if NETSTANDARD2_0
        public bool TryGetWriter(Type type, out INpoiExcelExportCellWriter? writer)
#else
        public bool TryGetWriter(Type type, [NotNullWhen(true)] out INpoiExcelExportCellWriter? writer)
#endif
        {
            foreach (var item in _writers)
            {
                if (item.CanWrite(type))
                {
                    writer = item;
                    return true;
                }
            }
            writer = null;
            return false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether a blank cell is kept when the exported value is null.
        /// When <see langword="false"/>, blank cells are removed.
        /// </summary>
        public bool CreateCellForNullValue { get; set; }
    }
}
