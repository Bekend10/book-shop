namespace book_shop.Dto
{
    public class CartDto
    {
        public int cart_id { get; set; }
        public int user_id { get; set; }
        public int total_amount { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }

        public List<CartDetailItemDto> items { get; set; }
    }

    public class CartDetailDto
    {
        public int cart_detail_id { get; set; }
        public int cart_id { get; set; }
        public int user_id { get; set; }
        public int total_amount { get; set; }
        public List<CartDetailDto> items { get; set; }
        public int book_id { get; set; }
        public int quantity { get; set; }
        public int unit_price { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }

    public class CartDetailItemDto
    {
        public int cart_detail_id { get; set; }
        public int book_id { get; set; }
        public int quantity { get; set; }

        public BookResponse book { get; set; }
    }

    public class AddToCartDto
    {
        public int book_id { get; set; }
        public int quantity { get; set; }
    }

    public class UpdateQuantityToCartDto
    {
        public int book_id { get; set; }
        public int quantity { get; set; }
    }
}
