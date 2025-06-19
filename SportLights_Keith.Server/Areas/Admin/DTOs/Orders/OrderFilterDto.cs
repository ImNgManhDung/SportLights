using SPORTLIGHTS_SERVER.Common.Base;

namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Orders
{
	public class OrderFilterDto : ViewPaginateInputBase

	{
		public string? SearchValue { get; set; } = string.Empty;

		public int Status { get; set; } = 0;
	}
}
