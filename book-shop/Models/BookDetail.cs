using System.ComponentModel.DataAnnotations;

namespace book_shop.Models
{
    public class BookDetail
    {
        [Key]
        public int detail_id { get; set; }
        public int book_id { get; set; }
        public int quantity { get; set; }
        public int price_origin { get; set; }
        public int is_bn { get; set; }
        public string description { get; set; } = string.Empty;
        public string image_url { get; set; } = string.Empty;
        public string file_demo_url { get; set; } = string.Empty;
        public string language { get; set; } = string.Empty;
        public string publisher { get; set; } = string.Empty;
        public int number_of_page { get; set; }
        public int price { get; set; }
        public DateTime create_at { get; set; }
        public DateTime publisher_year { get; set; }
        public Book book { get; set; } 
    }
}
