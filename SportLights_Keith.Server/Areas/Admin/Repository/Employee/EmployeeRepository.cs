using Dapper;
using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Employees;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Employees.Abstractions;
using SPORTLIGHTS_SERVER.Entities;
using SPORTLIGHTS_SERVER.Modules;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Repository.Employees
{
	public class EmployeeRepository : IEmployeeRepository
	{
		public int Count(EmployeeFilterDto filter)
		{
			int count = 0;
			string searchValue = filter.SearchValue ?? "";
			if (!string.IsNullOrEmpty(searchValue))
				searchValue = "%" + searchValue + "%";

			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var sql = @"SELECT COUNT(*) FROM Employees
								WHERE (@searchValue = N'') OR (FullName LIKE @searchValue)";
				count = connection.ExecuteScalar<int>(sql, new { searchValue });
			}

			return count;
		}

		public int CreateEmployee(CreateEmployeeDto dto)
		{
			int id = 0;
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var sql = @"IF EXISTS(SELECT * FROM Employees WHERE Email = @Email)
									SELECT -1
								ELSE
									BEGIN
										INSERT INTO Employees(FullName, BirthDate, Address, Phone, Email, Photo, IsWorking)
										VALUES(@FullName, @BirthDate, @Address, @Phone, @Email, @Photo, @IsWorking);
										SELECT CAST(SCOPE_IDENTITY() AS INT);
									END";
				var parameters = new
				{
					dto.FullName,
					dto.BirthDate,
					dto.Address,
					dto.Phone,
					dto.Email,
					dto.Photo,
					dto.IsWorking
				};

				id = connection.ExecuteScalar<int>(sql, parameters);
			}

			return id;
		}

		public bool DeleteEmployee(int id)
		{
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var sql = @"DELETE FROM Employees 
								WHERE EmployeeId = @EmployeeId 
								AND NOT EXISTS(SELECT * FROM Orders WHERE EmployeeId = @EmployeeId)";
				var affected = connection.Execute(sql, new { EmployeeId = id });
				return affected > 0;
			}
		}

		public Employee GetEmployeeById(int id)
		{
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var sql = @"SELECT * FROM Employees WHERE EmployeeId = @EmployeeId";
				return connection.QueryFirstOrDefault<Employee>(sql, new { EmployeeId = id });
			}
		}

		public List<Employee> GetEmployees(EmployeeFilterDto filter)
		{
			List<Employee> employees;
			string searchValue = filter.SearchValue ?? "";
			if (!string.IsNullOrEmpty(searchValue))
				searchValue = "%" + searchValue + "%";

			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var sql = @"
						WITH cte AS (
							SELECT EmployeeID, FullName, BirthDate, Address, Phone, Email, Photo, IsWorking,
								   ROW_NUMBER() OVER (ORDER BY FullName) AS RowNumber
							FROM Employees
							WHERE (@searchValue = N'') OR (FullName LIKE @searchValue)
						)
						SELECT * FROM cte
						WHERE (@PageSize = 0)
						   OR (RowNumber BETWEEN (@Page - 1) * @PageSize + 1 AND @Page * @PageSize)
						ORDER BY RowNumber";
				var parameters = new
				{
					Page = filter.Page,
					PageSize = filter.PageSize,
					searchValue = searchValue
				};

				employees = connection.Query<Employee>(sql, parameters).ToList();
			}

			return employees;
		}

		public bool UpdateEmployee(EditEmployeeDto dto)
		{
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var sql = @"IF NOT EXISTS(SELECT * FROM Employees 
											  WHERE EmployeeId <> @EmployeeId AND Email = @Email)
								BEGIN
									UPDATE Employees 
									SET FullName = @FullName,
										BirthDate = @BirthDate,
										Address = @Address,
										Phone = @Phone,
										Email = @Email,
										Photo = @Photo,
										IsWorking = @IsWorking
									WHERE EmployeeId = @EmployeeId
								END";

				var parameters = new
				{
					dto.EmployeeId,
					dto.FullName,
					dto.BirthDate,
					dto.Address,
					dto.Phone,
					dto.Email,
					dto.Photo,
					dto.IsWorking
				};

				return connection.Execute(sql, parameters) > 0;
			}
		}
	}
}
