using Dapper;
using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Employees;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Employees.Abstractions;
using SPORTLIGHTS_SERVER.Entities;
using SPORTLIGHTS_SERVER.Modules;
using System.Data;

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

		public async Task<int> CreateEmployee(CreateEmployeeDto dto)
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
					FullName =dto.FullName,
					BirthDate = dto.BirthDay,
					Address = dto.Address,
					Phone = dto.Phone,
					Email = dto.Email,
					Photo = dto.Photo,
					IsWorking = 1 // Default value for IsWorking
				};

				id = await connection.ExecuteScalarAsync<int>(sql, parameters);
			}

			return id;
		}

		public async Task<bool> DeleteEmployee(int employeeId)
		{
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var sql = @"DELETE FROM Employees 
								WHERE EmployeeId = @EmployeeId 
								AND NOT EXISTS(SELECT EmployeeId FROM Orders WHERE EmployeeId = @EmployeeId)";
				var affected = await connection.ExecuteAsync(sql, new { EmployeeId = employeeId });
				return affected > 0;
			}
		}

		public async Task<Employee?> GetEmployeeById(int employeeId)
		{
			Employee? customer;
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var sql = @"SELECT EmployeeId, FullName, BirthDate, Address, Phone, Email, Photo, IsWorking FROM Employees WHERE EmployeeId = @EmployeeId";
				 customer = await connection.QueryFirstOrDefaultAsync<Employee>(sql, new { EmployeeId = employeeId }, commandType: CommandType.Text);
			}
			return customer;
		}

		public async Task<IReadOnlyList<Employee>> LoadEmployees(EmployeeFilterDto filter)
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

				employees = (await connection.QueryAsync<Employee>(sql, parameters)).ToList();
			}

			return employees;
		}

		public async Task<bool> UpdateEmployee(EditEmployeeDto dto)
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
					EmployeeId = dto.EmployeeId,
					FullName = dto.FullName,
					BirthDate = dto.BirthDay,
					Address = dto.Address,
					Phone = dto.Phone,
					Email = dto.Email,
					Photo = dto.Photo,
					IsWorking = dto.IsWorking
				};

				return await connection.ExecuteAsync(sql, parameters) > 0;
			}
		}
	}
}
