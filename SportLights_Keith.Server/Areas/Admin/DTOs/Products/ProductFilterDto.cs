using SPORTLIGHTS_SERVER.Common.Base;

namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Products
{
	public class ProductFilterDto : ViewPaginateInputBase
	{
		public string? SearchValue { get; set; } = string.Empty;



	}
}
