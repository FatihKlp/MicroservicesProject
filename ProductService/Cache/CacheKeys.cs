namespace ProductService.Cache
{
    public static class CacheKeys
    {
        public const string ProductsAll = "products_all";

        public static string ProductById(int id) => $"product_{id}";
    }
}
