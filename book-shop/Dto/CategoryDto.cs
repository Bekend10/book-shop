namespace book_shop.Dto
{
    public class CategoryDto
    {
        public int category_id { get; set; }
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public DateTime created_at { get; set; } = DateTime.Now;
        public int created_by { get; set; }
    }
}
