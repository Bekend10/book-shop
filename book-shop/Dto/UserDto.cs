namespace book_shop.Dto
{
    public class UserDto
    {
        public int user_id { get; set; }
        public string first_name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
    }
}
