using book_shop.Enums;
using book_shop.Models;
using static book_shop.Enums.OrderEnumStatus;

namespace book_shop.Dto
{
    public class OrderDto
    {
        public int order_id { get; set; }
        public int user_id { get; set; }
        public DateTime order_date { get; set; }
        public OrderStatus status { get; set; }
        public int total_amount { get; set; }
        public int method_id { get; set; }
        public int quantity { get; set; }
        public int book_id { get; set; }
        public int unit_price { get; set; }
    }

    public class UpdateOrderDto
    {
        public OrderStatus status { get; set; }
    }

    public class OrderRespone
    {
        public int order_id { get; set; }
        public User user { get; set; }
        public Address address { get; set; }
        public BookResponseDto items { get; set; }
        public OrderDetail orderDetail { get; set; }
        public DateTime order_date { get; set; }
        public OrderStatus status { get; set; }
        public int total_amount { get; set; }
        public Method method { get; set; }
        public PaymentEnumStatus payment { get; set; }
        public int method_id { get; set; }
        public int quantity { get; set; }
        public int book_id { get; set; }
        public int unit_price { get; set; }
    }
}
