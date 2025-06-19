using SPORTLIGHTS_SERVER.Common.Base;

namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Shippers
{
	public class ShipperFilterDto : ViewPaginateInputBase
	{
		public string? SearchValue { get; set; } = string.Empty;



	}
}
