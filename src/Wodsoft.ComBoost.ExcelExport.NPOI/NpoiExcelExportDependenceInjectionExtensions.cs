using Wodsoft.ComBoost.ExcelExport;
using Wodsoft.ComBoost.ExcelExport.NPOI;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Registers NPOI Excel export services.
    /// </summary>
    public static class NpoiExcelExportDependenceInjectionExtensions
    {
        /// <summary>
        /// Adds <see cref="NpoiExcelExportService"/> as the singleton <see cref="IExcelExportService"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The same service collection.</returns>
        public static IServiceCollection AddNpoiExcelExport(this IServiceCollection services)
        {
            return services.AddSingleton<IExcelExportService, NpoiExcelExportService>();
        }
    }
}
