using System.ComponentModel.DataAnnotations;

namespace book_shop.Models
{
    public class Address
    {
        [Key]
        public int address_id { get; set; }
        public string? country { get; set; } = string.Empty;
        public string? councious { get; set; } = string.Empty ;
        public string? district { get; set; } = string.Empty;
        public string? commune { get; set; } = string.Empty;
        public string? house_number { get; set; } = string.Empty;
        public User User { get; set; }
    }
}
