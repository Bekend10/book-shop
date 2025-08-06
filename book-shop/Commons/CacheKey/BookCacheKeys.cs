namespace book_shop.Commons.CacheKey
{
    public static class BookCacheKeys
    {
        public const string _bookList = "book:list";
        public static string BookById(int bookId) => $"book:id:{bookId}";
        public static string BookByCategory(int categoryId) => $"book:category:{categoryId}";
        public static string BookByAuthor(int authorId) => $"book:author:{authorId}";
        public static string BookByTitle(string title) => $"book:title:{title}";
        public static string BookByPublisher(string publisher) => $"book:publisher:{publisher}";
        public static string TopProducts(DateTime? startDate , DateTime? endDate) => $"book:top-products:{startDate}:{endDate}";
    }
}
