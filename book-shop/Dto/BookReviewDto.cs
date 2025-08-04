namespace book_shop.Dto
{
    public class BookReviewDto
    {
        public int book_review_id { get; set; }
        public int book_id { get; set; }
        public int user_id { get; set; }
        public double rating { get; set; }
        public string? content { get; set; }
        public string? image { get; set; }

    }
    
    public class BookReviewCreateDto
    {
        public int book_id { get; set; }
        public double rating { get; set; }
        public string? content { get; set; }
        public IFormFile? image { get; set; } 
    }

    public class BookReviewUpdateDto
    {
        public int book_review_id { get; set; }
        public int book_id { get; set; }
        public int user_id { get; set; }
        public double rating { get; set; }
        public string? content { get; set; }
        public IFormFile? image { get; set; }
    }

    public class BookReviewResponseDto
    {
        public int book_review_id { get; set; }
        public int book_id { get; set; }
        public int user_id { get; set; }
        public double rating { get; set; }
        public string? content { get; set; }
        public DateTime created_at { get; set; }
        public BookResponseDto Book { get; set; }
        public UserRespone User { get; set; }
        public string? image { get; set; }
    }
}
