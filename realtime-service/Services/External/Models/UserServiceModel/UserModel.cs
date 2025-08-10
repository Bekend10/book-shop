namespace realtime_service.Services.External.Models.UserServiceModel
{
    public class AddressModel
    {
        public int AddressId { get; set; }
        public string Country { get; set; }
        public string Councious { get; set; }
        public string District { get; set; }
        public string Commune { get; set; }
        public string HouseNumber { get; set; }
    }

    public class UserModel
    {
        public int AccountId { get; set; }
        public int UserId { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Email { get; set; }
        public string Phone_Number { get; set; }
        public AddressModel Address { get; set; }
        public string Profile_Image { get; set; }
        public string Full_Name { get; set; }
        public string? Role { get; set; }
    }
}
