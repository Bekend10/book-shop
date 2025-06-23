namespace book_shop.Dto
{
    public class AuthorDto
    {
        public int author_id { get; set; }
        public string name { get; set; } = string.Empty;
        public string bio { get; set; } = string.Empty;
        public DateTime dob { get; set; }
        public string nationally { get; set; } = string.Empty;
    }

    public class UpdateAuthorDto
    {
        public string? name { get; set; } = string.Empty;
        public string? bio { get; set; } = string.Empty;
        public DateTime? dob { get; set; }
        public string? nationally { get; set; } = string.Empty;
    }
}
