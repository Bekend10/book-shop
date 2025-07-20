namespace book_shop.Dto
{
    public class CreateUserDto
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public DateTime dob { get; set; }
        public bool gender { get; set; }
        public string profile_image { get; set; }
        public int address_id { get; set; }
    }

    public class CreateUserByAdmin
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public string password { get; set; }
        public int role_id { get; set; }
        public int is_verify { get; set; }
        public bool is_active { get; set; }
    }
}
