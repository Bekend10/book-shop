using book_shop.Models;

namespace book_shop.Dto
{
    public class UserDto
    {
        public int user_id { get; set; }
        public string first_name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
    }

    public class UserRespone
    {
        public int account_id { get; set; }
        public int user_id { get; set; }
        public string? first_name { get; set; } = string.Empty;
        public string? last_name { get; set; } = string.Empty;
        public string? email { get; set; } = string.Empty;
        public string? phone_number { get; set; } = string.Empty;
        public Address address { get; set; }
        public string? profile_image { get; set; } = string.Empty;
        public string? full_name { get; set; } = string.Empty;
        public string? role { get; set; }
    }
}
