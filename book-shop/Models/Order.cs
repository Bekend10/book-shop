using System.ComponentModel.DataAnnotations;
using static book_shop.Enums.OrderEnumStatus;

namespace book_shop.Models
{
    public class Order
    {
        [Key]
        public int order_id { get; set; }
        public int user_id { get; set; }
        public DateTime order_date { get; set; }
        public OrderStatus status { get; set; }
        public int total_amount { get; set; }
        public int method_id { get; set; }
        public Method method { get; set; }
        public OrderDetail orderDetail { get; set; }
        public User User { get; set; }
    }
}
