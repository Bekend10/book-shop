namespace book_shop.Models
{
    public class VnpayPaymentRequest
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string OrderInfo { get; set; }
    }
}
