using SPORTLIGHTS_SERVER.Common.Base;
using SPORTLIGHTS_SERVER.Entities;

namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Products
{
	public class PaginatedProductDto : ViewPaginateOutputBase<Product>
	{
		public string? SearchValue { get; set; } = string.Empty;
	}

}
