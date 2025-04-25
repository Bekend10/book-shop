namespace book_shop.Dto
{
    public class UpdateUserDto
    {
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public DateTime? dob { get; set; }
        public bool? gender { get; set; }
        public string? profile_image { get; set; }
        public int? address_id { get; set; }
    }
}
