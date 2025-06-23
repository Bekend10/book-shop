namespace book_shop.Enums
{
    public class OrderEnumStatus
    {
        public enum OrderStatus
        {
            Pending,
            Processing,
            Shipped,
            Delivered,
            Cancelled
        }

        public enum PaymentStatus
        {
            Pending,
            Completed,
            Failed,
            Refunded
        }
    }
}
