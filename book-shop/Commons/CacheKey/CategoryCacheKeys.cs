namespace book_shop.Commons.CacheKey
{
    public static class CategoryCacheKeys
    {
        public static string GetAllCategories => "category:getAllCategories";
        public static string GetCategoryById(int id) => $"category:getCategoryById:{id}";
        public static string GetCategoryByName(string name) => $"category:getCategoryByName:{name}";
        public static string GetBooksByCategoryId(int categoryId) => $"category:getBooksByCategoryId:{categoryId}";
    }
}
