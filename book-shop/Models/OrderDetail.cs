using System.ComponentModel.DataAnnotations;

namespace book_shop.Models
{
    public class OrderDetail
    {
        [Key]
        public int order_detail_id { get; set; }
        public int order_id { get; set; }
        public int book_id { get; set; }
        public int quantity { get; set; }
        public int unit_price { get; set; }
        public Order order { get; set; }
        public Book book { get; set; }   
    }
}
