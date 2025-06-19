using SPORTLIGHTS_SERVER.Common.Base;
using SPORTLIGHTS_SERVER.Entities;
namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Employees
{
	public class PaginatedEmployeeDto : ViewPaginateOutputBase<Employee>
	{
		public string? SearchValue { get; set; } = string.Empty;
	}
}
