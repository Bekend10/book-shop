namespace book_shop.Services.Interfaces
{
    public interface IRevenueService
    {
         Task<object> GetRevenueAsync(DateTime? startDate, DateTime? endDate);
    }
}
