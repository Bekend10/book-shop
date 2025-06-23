using book_shop.Models;

namespace book_shop.Dto
{
    public class BookResponseDto
    {
        public int book_id { get; set; }
        public int category_id { get; set; }
        public int author_id { get; set; }
        public string title { get; set; } = string.Empty;
        public string publisher { get; set; } = string.Empty;
        public DateTime publisher_year { get; set; }
        public int is_bn { get; set; }
        public int quantity { get; set; }
        public DateTime create_at { get; set; }
        public string description { get; set; } = string.Empty;
        public string image_url { get; set; } = null!;
        public string file_demo_url { get; set; } = string.Empty;
        public string language { get; set; } = string.Empty;
        public int number_of_page { get; set; }
        public int price { get; set; }
        public BookDetail BookDetail { get; set; } = null!;
        public Author author { get; set; } = null!;
    }


    public class UpdateBookDto
    {
        public string? title { get; set; }
        public int? author_id { get; set; }
        public string? publisher { get; set; }
        public int? price { get; set; }
        public int? category_id { get; set; }
        public DateTime? publisher_year { get; set; }
        public int? quantity { get; set; }
        public int? is_bn { get; set; }

        public IFormFile? image { get; set; }

        // Chi tiết
        public string? description { get; set; }
        public string? language { get; set; }
        public int? number_of_page { get; set; }
        public string? file_demo_url { get; set; }
    }
}
