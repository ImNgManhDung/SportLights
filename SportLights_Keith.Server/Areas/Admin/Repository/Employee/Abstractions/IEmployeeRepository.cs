using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Employees;
using SPORTLIGHTS_SERVER.Entities;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Repository.Employees.Abstractions
{
	public interface IEmployeeRepository
	{
		int Count(EmployeeFilterDto filter);
		Task<IReadOnlyList<Employee>> LoadEmployees(EmployeeFilterDto filter);
		Task<Employee?> GetEmployeeById(int employeeId);
		Task<int> CreateEmployee(CreateEmployeeDto dto);
		Task<bool> UpdateEmployee(EditEmployeeDto dto);
		Task<bool> DeleteEmployee(int employeeId);
	}

}
