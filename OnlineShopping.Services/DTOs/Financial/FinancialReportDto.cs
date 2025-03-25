namespace OnlineShopping.Services.DTOs.Financial;

public class FinancialReportDto
{
    public decimal TotalRevenue { get; set; }
    public int TotalOrderCount { get; set; }
    public decimal AverageOrderValue { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}