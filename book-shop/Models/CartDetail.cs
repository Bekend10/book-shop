using System.ComponentModel.DataAnnotations;

namespace book_shop.Models
{
    public class CartDetail
    {
        [Key]
        public int cart_detail_id { get; set; }
        public int cart_id { get; set; }
        public int book_id { get; set; }
        public int quantity { get; set; }
        public int unit_price { get; set; }
        public Cart cart { get; set; }
        public Book book { get; set; }
    }
}
