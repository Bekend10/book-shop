namespace book_shop.Commons.CacheKey
{
    public static class AuthorCacheKeys
    {
        public const string _authorList = "author:list";
        public static string AuthorById(int authorId) => $"author:id:{authorId}";
        public static string AuthorByName(string name) => $"author:name:{name}";
        public static string AuthorByBook(int bookId) => $"author:book:{bookId}";
        public static string AuthorByCategory(int categoryId) => $"author:category:{categoryId}";
    }
}
