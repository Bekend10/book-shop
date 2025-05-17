using System.ComponentModel.DataAnnotations;

namespace book_shop.Models
{
    public class Book
    {
        [Key]
        public int book_id { get; set; }
        public string title { get; set; } = string.Empty;
        public int author_id { get; set; } 
        public DateTime publisher_year { get; set; }
        public string publisher { get; set; } = string.Empty;
        public string image_url { get; set; } = string.Empty;
        public int is_bn { get; set; }
        public int quantity { get; set; }
        public int price { get; set; }
        public DateTime created_at { get; set; }
        public int category_id { get; set; }
        public Category category { get; set; } 
        public BookDetail bookDetail { get; set; }
        public ICollection<Author> authors { get; set; }
    }
}
