using book_shop.Enums;
using book_shop.Models;

namespace book_shop.Dto
{
    public class MonthlyRevenueDto
    {
        public int month { get; set; }
        public int revenue { get; set; }
    }

    public class RevenueByCategoryDto
    {
        public string category_name { get; set; }
        public int total_revenue { get; set; }
    }

    public class ListOrderDto
    {
        public int order_id { get; set; }
        public DateTime created_at { get; set; }
        public string customer_name { get; set; }
        public string customer_email { get; set; }
        public int total_amount { get; set; }
        public string status { get; set; }
    }

    public class TopProductDto
    {
        public int product_id { get; set; }
        public string name { get; set; }
        public string category_name { get; set; }
        public int quantity_sold { get; set; }
        public int revenue { get; set; }
        public string image { get; set; }
        public Author author { get; set; }
    }
    public class RevenueDto
    {
        public int total_revenue { get; set; }
        public int total_orders { get; set; }
        public int avg_order_value { get; set; }
        public int total_products_sold { get; set; }
        public List<MonthlyRevenueDto> monthly_revenue { get; set; }
        public List<TopProductDto> top_products { get; set; }
        public List<RevenueByCategoryDto> revenue_by_category { get; set; }
        public List<ListOrderDto> orders { get; set; }
    }
}
