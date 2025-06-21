using System.ComponentModel.DataAnnotations;

namespace book_shop.Models
{
    public class Cart
    {
        [Key]
        public int cart_id { get; set; }
        public int user_id { get; set; }
        public int total_amount { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public User User { get; set; }
        public ICollection<CartDetail> cart_detail { get; set; }
    }
}
