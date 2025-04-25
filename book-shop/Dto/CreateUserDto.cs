namespace book_shop.Dto
{
    public class CreateUserDto
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public DateTime dob { get; set; }
        public bool gender { get; set; }
        public string profile_image { get; set; }
        public int address_id { get; set; }
    }

}
