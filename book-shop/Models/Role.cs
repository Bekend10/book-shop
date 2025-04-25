using System.ComponentModel.DataAnnotations;

namespace book_shop.Models
{
    public class Role
    {
        [Key]
        public int role_id { get; set; }
        public string role_name { get; set; } = string.Empty;
        public ICollection<Account> account { get; set; }
    }
}
