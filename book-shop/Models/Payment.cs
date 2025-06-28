using book_shop.Enums;
using System.ComponentModel.DataAnnotations;

namespace book_shop.Models
{
    public class Payment
    {
        [Key]
        public int payment_id { get; set; }
        public int order_id { get; set; }
        public int method_id { get; set; }
        public DateTime payment_date { get; set; }
        public int amount { get; set; }
        public PaymentEnumStatus payment_status{ get; set; }
        public Order order { get; set; }
        public Method method { get; set; }
    }
}
