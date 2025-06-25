using SPORTLIGHTS_SERVER.Common.Base;
using SPORTLIGHTS_SERVER.Entities;

namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Categories
{
	public class ViewCategory : ViewPaginateOutputBase<Category>
	{
		public string? SearchValue { get; set; } = string.Empty;

	}
}
