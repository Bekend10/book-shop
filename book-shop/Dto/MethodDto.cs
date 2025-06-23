namespace book_shop.Dto
{
    public class MethodDto
    {
        public int method_id { get; set; }
        public string method_name { get; set; }
        public string description { get; set; }
    }

    public class UpdateMethodDto
    {
        public string method_name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
    }
}
