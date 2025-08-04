using System.ComponentModel.DataAnnotations;

namespace book_shop.Models
{
    public class BookReview
    {
        [Key]
        public int book_review_id { get; set; }
        public int book_id { get; set; }
        public int user_id { get; set; }
        public double rating { get; set; }
        public string? content { get; set; }
        public string? image { get; set; }
        public DateTime created_at { get; set; }
        public Book book { get; set; }
        public User user { get; set; }
    }
}
