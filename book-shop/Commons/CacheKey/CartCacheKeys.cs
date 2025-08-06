namespace book_shop.Commons.CacheKey
{
    public static class CartCacheKeys
    {
        public static string myCart(int userId) => $"cart:myCart:{userId}";
    }
}
