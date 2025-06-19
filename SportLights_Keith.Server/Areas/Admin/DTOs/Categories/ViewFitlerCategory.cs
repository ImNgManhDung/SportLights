using SPORTLIGHTS_SERVER.Common.Base;

namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Categories
{
	public class ViewFitlerCategory : ViewPaginateInputBase
	{
		public string? SearchValue { get; set; } = string.Empty;



	}
}
