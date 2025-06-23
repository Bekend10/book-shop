namespace book_shop.Dto
{
    public class BookDetailDto
    {
        public int book_id { get; set; }
        public string description { get; set; } = string.Empty;
        public string image_url { get; set; } = string.Empty;
        public string file_demo_url { get; set; } = string.Empty;
        public string language { get; set; } = string.Empty;
        public string publisher { get; set; } = string.Empty;
        public int price { get; set; }  
        public int number_of_page { get; set; }
        public int is_bn { get; set; }
        public int quantity { get; set; }
        public DateTime create_at { get; set; }
        public DateTime publisher_year { get; set; }
    }
}
