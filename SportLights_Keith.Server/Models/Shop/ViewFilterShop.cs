using SPORTLIGHTS_SERVER.Common.Base;

namespace SPORTLIGHTS_SERVER.Models.Shop
{
	public class ViewFilterShop : ViewPaginateInputBase
	{
		public string? SortField { get; set; }

		public int SortType { get; set; }

		public int CatelogProductsID { get; set; }

		public string? SearchValue { get; set; }
	}
}
