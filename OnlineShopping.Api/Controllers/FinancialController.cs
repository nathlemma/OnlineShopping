using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Services.DTOs.Financial;
using OnlineShopping.Services.Interfaces;

namespace OnlineShopping.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinancialController : ControllerBase
    {
        private readonly IFinancialService _financialService;

        public FinancialController(IFinancialService financialService)
        {
            _financialService = financialService;
        }

        [HttpGet("report")]
        public async Task<ActionResult<FinancialReportDto>> GetFinancialReport(
            [FromQuery] DateTime? startDate = null, 
            [FromQuery] DateTime? endDate = null)
        {
            var report = await _financialService.GenerateFinancialReport(startDate, endDate);
            return Ok(report);
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportFinancialReport(
            [FromQuery] DateTime? startDate = null, 
            [FromQuery] DateTime? endDate = null)
        {
            var excelBytes = await _financialService.ExportFinancialReportToExcel(startDate, endDate);
            
            string fileName = "FinancialReport";
            if (startDate.HasValue) fileName += $"_{startDate.Value:yyyy-MM-dd}";
            if (endDate.HasValue) fileName += $"_to_{endDate.Value:yyyy-MM-dd}";
            
            fileName += ".xlsx";
            
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}