using book_shop.Dto;

namespace book_shop.Services.Implementations
{
    public interface IReportService
    {
        Task<byte[]> ExportOrderReportAsync(OrderRequestModel query);
    }
}
