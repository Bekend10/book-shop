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

    public class UserGoogleDto
    {
        public int id { get; set; }
        public string google_id { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string profile_img { get; set; }
        public string phone_number { get; set; }
        public int address_id { get; set; }
        public DateTime created_at { get; set; }
    }

    public class UserFaceBookDto
    {
        public string id { get; set; }
        public string facebook_id { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string profile_img { get; set; }
        public string phone_number { get; set; }
        public int address_id { get; set; }
        public DateTime created_at { get; set; }
    }

    public class UserFaceBookRespone
    {
        public string id { get; set; }
        public string facebook_id { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public PictureData picture { get; set; }
        public string phone_number { get; set; }
        public int address_id { get; set; }
        public string profile_img => picture?.Data?.Url;
    }

    public class PictureData
    {
        public PictureInfo Data { get; set; }
    }

    public class PictureInfo
    {
        public string Url { get; set; }
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
