using System.ComponentModel.DataAnnotations;

namespace book_shop.Models
{
    public class Method
    {
        [Key]
        public int method_id { get; set; }
        public string method_name { get; set; }
        public string description { get; set; }
        public ICollection<Order> orders { get; set; }
        public ICollection<Payment> payments { get; set; }
    }
}
