using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Employees;
using SPORTLIGHTS_SERVER.Entities;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Repository.Employees.Abstractions
{
	public interface IEmployeeRepository
	{
		int Count(EmployeeFilterDto filter);
		List<Employee> GetEmployees(EmployeeFilterDto filter);
		Employee GetEmployeeById(int id);
		int CreateEmployee(CreateEmployeeDto dto);
		bool UpdateEmployee(EditEmployeeDto dto);
		bool DeleteEmployee(int id);
	}

}
