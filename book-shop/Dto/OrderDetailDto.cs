namespace book_shop.Dto
{
    public class OrderDetailDto
    {
        public int order_detail_id { get; set; }
        public int order_id { get; set; }
        public int book_id { get; set; }
        public int quantity { get; set; }
        public int unit_price { get; set; }
    }

    public class UpdateOrderDetailDto
    {
        public int book_id { get; set; }
        public int quantity { get; set; }
        public int unit_price { get; set; }
    }
}
