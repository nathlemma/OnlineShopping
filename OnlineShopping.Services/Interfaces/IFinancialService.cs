using OnlineShopping.Services.DTOs.Financial;

namespace OnlineShopping.Services.Interfaces
{
    public interface IFinancialService
    {
        Task<FinancialReportDto> GenerateFinancialReport(DateTime? startDate = null, DateTime? endDate = null);
        Task<byte[]> ExportFinancialReportToExcel(DateTime? startDate = null, DateTime? endDate = null);
    }
}