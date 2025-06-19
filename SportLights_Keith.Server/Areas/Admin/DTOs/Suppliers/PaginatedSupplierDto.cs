using SPORTLIGHTS_SERVER.Common.Base;
using SPORTLIGHTS_SERVER.Entities;

namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Suppliers
{
	public class PaginatedSupplierDto : ViewPaginateOutputBase<Supplier>
	{
		public string? SearchValue { get; set; } = string.Empty;
	}

}

