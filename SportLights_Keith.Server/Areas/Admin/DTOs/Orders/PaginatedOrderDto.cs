using SPORTLIGHTS_SERVER.Common.Base;
using SPORTLIGHTS_SERVER.Entities;

namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Orders
{
	public class PaginatedOrderDto : ViewPaginateOutputBase<Order>
	{
		public string? SearchValue { get; set; } = string.Empty;
	}
}
