namespace book_shop.Dto
{
    public class ChangePasswordDto
    {
        public int account_id { get; set; }
        public string old_password { get; set; } = string.Empty;
        public string new_password { get; set; } = string.Empty;
    }
}
