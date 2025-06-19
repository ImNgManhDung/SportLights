using SPORTLIGHTS_SERVER.Common.Base;
using SPORTLIGHTS_SERVER.Entities;

namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Customers
{
	public class PaginatedCustomerDto : ViewPaginateOutputBase<Customer>
	{
		public string? SearchValue { get; set; } = string.Empty;
	}
}
