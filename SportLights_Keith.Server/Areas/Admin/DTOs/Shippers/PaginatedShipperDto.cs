using SPORTLIGHTS_SERVER.Common.Base;
using SPORTLIGHTS_SERVER.Entities;

namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Shippers
{
	public class PaginatedShipperDto : ViewPaginateOutputBase<Shipper>
	{
		public string? SearchValue { get; set; } = string.Empty;
	}

}