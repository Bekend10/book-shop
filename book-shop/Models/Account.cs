using System.ComponentModel.DataAnnotations;

namespace book_shop.Models
{
    public class Account
    {
        [Key]
        public int account_id { get; set; }
        public string email { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string refresh_token { get; set; } = string.Empty;
        public DateTime refresh_token_ext { get; set; }
        public int user_id { get; set; }
        public User user { get; set; }
        public int role_id { get; set; }
        public int is_verify { get; set; }
        public bool is_active { get; set; }
        public Role role { get; set; }
    }
}
