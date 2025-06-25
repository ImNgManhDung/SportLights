namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Employees
{
	public class EditEmployeeDto : CreateEmployeeDto
	{
		public int EmployeeId { get; set; }

		public bool  IsWorking { get; set; }
	}
}
