namespace payment_service.Models
{
    public class VnpayPaymentRequest
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string OrderInfo { get; set; }
    }
}
