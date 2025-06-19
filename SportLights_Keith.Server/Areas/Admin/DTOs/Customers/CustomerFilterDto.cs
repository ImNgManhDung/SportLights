using SPORTLIGHTS_SERVER.Common.Base;

namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Customers
{
	public class CustomerFilterDto : ViewPaginateInputBase
	{
		public string? SearchValue { get; set; } = string.Empty;
	}
}
