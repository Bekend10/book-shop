using System.ComponentModel.DataAnnotations;

namespace book_shop.Models
{
    public class User
    {
        [Key]
        public int user_id { get; set; }
        public string? google_id { get; set; }
        public string? facebook_id { get; set; }
        public string first_name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string phone_number { get; set; }
        public DateTime created_at { get; set; }
        public string profile_image {  get; set; } = string.Empty;
        public int address_id { get; set; }
        public int book_review_id { get; set; }
        public bool gender { get; set; }
        public DateTime dob { get; set; }
        public Account Account { get; set; }
        public Address Address { get; set; }
        public Cart Cart { get; set; }
        public ICollection<Order> Order { get; set; }
        public ICollection<BookReview> BookReview { get; set; }
    }
}
