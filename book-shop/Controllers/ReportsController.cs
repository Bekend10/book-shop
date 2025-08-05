using book_shop.Dto;
using book_shop.Services.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace book_shop.Controllers
{
    [Route("api/v1/reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("export-order")]
        public async Task<IActionResult> ExportExcel([FromQuery] OrderRequestModel query)
        {
            var bytes = await _reportService.ExportOrderReportAsync(query);
            var fileName = $"BaoCaoDonHang_{DateTime.Now:yyyyMMdd}.xlsx";
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
