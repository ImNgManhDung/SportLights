using SPORTLIGHTS_SERVER.Areas.Admin.Controllers;

namespace SPORTLIGHTS_SERVER.Modules
{
	public static class CacheKeyHelper
	{
		public static string Category(string searchValue, int page)
			=> $"category:{searchValue}:{page}";
		public static string Customer(string searchValue, int page)
			=> $"customer:{searchValue}:{page}";

		public static string Product(string searchValue, int page)
			=> $"product:{searchValue}:{page}";
		public static string Order(string searchValue, int page)
			=> $"order:{searchValue}:{page}";

		public static string Employee(string searchValue, int page)
			=> $"employee:{searchValue}:{page}";
		public static string Shipper(string searchValue, int page)
			=> $"shipper:{searchValue}:{page}";

		public static string Supplier(string searchValue, int page)
			=> $"supplier:{searchValue}:{page}";

	}

	public static class CachePrefixes
	{
		public const string Category = "category";
		public const string Customer = "customer";
		public const string Product = "product";
		public const string Order = "order";
		public const string Employee = "employee";
		public const string Shipper = "shipper";
		public const string Supplier = "supplier";
	}

}
