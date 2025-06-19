using SPORTLIGHTS_SERVER.Common.Base;

namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Employees
{
	public class EmployeeFilterDto : ViewPaginateInputBase
	{
		public string? SearchValue { get; set; } = string.Empty;
	}

}
