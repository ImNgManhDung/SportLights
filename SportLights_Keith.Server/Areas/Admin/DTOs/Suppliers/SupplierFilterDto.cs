using SPORTLIGHTS_SERVER.Common.Base;

namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Suppliers
{
	public class SupplierFilterDto : ViewPaginateInputBase
	{
		public string? SearchValue { get; set; } = string.Empty;



	}
}
