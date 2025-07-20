namespace book_shop.Dto
{
    public class UpdateUserDto
    {
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public DateTime? dob { get; set; }
        public bool? gender { get; set; }
        public string? profile_image { get; set; }
        public string? phone_number { get; set; }
        public string country { get; set; } = string.Empty;
        public string councious { get; set; } = string.Empty;
        public string district { get; set; } = string.Empty;
        public string commune { get; set; } = string.Empty;
        public string house_number { get; set; } = string.Empty;
    }

    public class UpdateUserByAdmin
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public int role_id { get; set; }
        public bool is_active { get; set; }
    }
}
