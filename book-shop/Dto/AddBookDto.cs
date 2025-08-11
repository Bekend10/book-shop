namespace book_shop.Dto
{
    public class AddBookDto
    {
        public int book_id { get; set; }
        public int category_id { get; set; }
        public int author_id { get; set; }
        public string title { get; set; } = string.Empty;
        public string publisher { get; set; } = string.Empty;
        public DateTime publisher_year { get; set; }
        public int is_bn { get; set; }
        public int quantity { get; set; }
        public int price_origin { get; set; }
        public DateTime create_at { get; set; }
        public string description { get; set; } = string.Empty;
        public IFormFile image { get; set; } = null!;
        public string file_demo_url { get; set; } = string.Empty;
        public string language { get; set; } = string.Empty;
        public int number_of_page { get; set; }
        public int price { get; set; }
    }

    public class AddBookByExcelModel
    {
        public string title { get; set; }
        public DateTime publisher_year { get; set; }
        public string publisher { get; set; }
        public string image_url { get; set; }
        public int is_bn { get; set; }
        public int quantity { get; set; }
        public int price { get; set; }
        public int price_origin { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
        public string category_name { get; set; }
        public string author_name { get; set; }
        public int category_id { get; set; }
        public int author_id { get; set; }
        public BookDetail bookDetail { get; set; }
    }

    public class BookDetail
    {
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
    }
    public class ImportBooksResult
    {
        public int TotalProcessed { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
    }
}
