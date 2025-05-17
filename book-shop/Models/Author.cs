using System.ComponentModel.DataAnnotations;

namespace book_shop.Models
{
    public class Author
    {
        [Key]
        public int author_id { get; set; }
        public string name  { get; set; } = string.Empty;
        public string bio { get; set; } = string.Empty;
        public DateTime dob { get; set; }
        public string nationally { get; set; } = string.Empty;
        public ICollection<Book> books { get; set; }
    }
}
