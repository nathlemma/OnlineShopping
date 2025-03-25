using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OnlineShopping.Datahub;
using OnlineShopping.Services.DTOs.Financial;
using OnlineShopping.Services.Interfaces;


namespace OnlineShopping.Services.BusinessLogic
{
    public class FinancialService : IFinancialService
    {
        private readonly OnlineShoppingDbContext _context;

        public FinancialService(OnlineShoppingDbContext context)
        {
            _context = context;
        }

        public async Task<FinancialReportDto> GenerateFinancialReport(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Orders
                .Where(o => o.IsActive);
            
            if (startDate.HasValue)
                query = query.Where(o => o.OrderDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(o => o.OrderDate <= endDate.Value);

            var orders = await query.ToListAsync();

            decimal totalRevenue = orders.Sum(o => o.TotalAmount);
            int totalOrderCount = orders.Count;
            decimal avgOrderValue = totalOrderCount > 0 ? totalRevenue / totalOrderCount : 0;

            return new FinancialReportDto
            {
                TotalRevenue = totalRevenue,
                TotalOrderCount = totalOrderCount,
                AverageOrderValue = avgOrderValue,
                StartDate = startDate,
                EndDate = endDate
            };
        }

        public async Task<byte[]> ExportFinancialReportToExcel(DateTime? startDate = null, DateTime? endDate = null)
        {
            var report = await GenerateFinancialReport(startDate, endDate);
            
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet("Financial Report");
            
            var headerStyle = workbook.CreateCellStyle();
            var headerFont = workbook.CreateFont();
            headerFont.IsBold = true;
            headerStyle.SetFont(headerFont);

            var currencyStyle = workbook.CreateCellStyle();
            var currencyFormat = workbook.CreateDataFormat();
            currencyStyle.DataFormat = currencyFormat.GetFormat("$#,##0.00");
            
            var titleRow = sheet.CreateRow(0);
            var cell = titleRow.CreateCell(0);
            cell.SetCellValue("Financial Report");
            cell.CellStyle = headerStyle;
            
            var dateRangeRow = sheet.CreateRow(1);
            dateRangeRow.CreateCell(0).SetCellValue("Period:");
            dateRangeRow.CreateCell(1).SetCellValue(
                $"{(report.StartDate.HasValue ? report.StartDate.Value.ToString("yyyy-MM-dd") : "All time")} to " +
                $"{(report.EndDate.HasValue ? report.EndDate.Value.ToString("yyyy-MM-dd") : "Present")}");
            
            var headerRow = sheet.CreateRow(3);
            headerRow.CreateCell(0).SetCellValue("Metric");
            headerRow.CreateCell(1).SetCellValue("Value");
            
            var totalRevenueRow = sheet.CreateRow(4);
            totalRevenueRow.CreateCell(0).SetCellValue("Total Revenue");
            var totalRevenueCell = totalRevenueRow.CreateCell(1);
            totalRevenueCell.SetCellValue((double)report.TotalRevenue);
            totalRevenueCell.CellStyle = currencyStyle;
            
            var orderCountRow = sheet.CreateRow(5);
            orderCountRow.CreateCell(0).SetCellValue("Total Order Count");
            orderCountRow.CreateCell(1).SetCellValue(report.TotalOrderCount);
            
            var avgOrderRow = sheet.CreateRow(6);
            avgOrderRow.CreateCell(0).SetCellValue("Average Order Value");
            var avgOrderCell = avgOrderRow.CreateCell(1);
            avgOrderCell.SetCellValue((double)report.AverageOrderValue);
            avgOrderCell.CellStyle = currencyStyle;
            
            for (int i = 0; i < 2; i++)
            {
                sheet.AutoSizeColumn(i);
            }
            
            using (var memoryStream = new MemoryStream())
            {
                workbook.Write(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}