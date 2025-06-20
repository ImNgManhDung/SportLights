using Dapper;
using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Customers;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Customers.Abstractions;
using SPORTLIGHTS_SERVER.Entities;
using SPORTLIGHTS_SERVER.Modules;
using System.Data;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Repository.CustomerRepository
{
	public class CustomerRepository : ICustomerRepository
	{
		public int Count(CustomerFilterDto filter)
		{
			int count = 0;
			string searchValue = filter.SearchValue ?? "";
			if (!string.IsNullOrEmpty(searchValue))
				searchValue = "%" + searchValue + "%";

			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var sql = @"SELECT COUNT(*) FROM Customers 
                            WHERE (@searchValue = N'') OR (CustomerName LIKE @searchValue)";
				var parameters = new { searchValue };
				count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
			}
			return count;
		}

		public async Task<int> CreateCustomer(CreateCustomerDto dto)
		{
			int id = 0;
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var sql = @"IF EXISTS (SELECT * FROM Customers WHERE Email = @Email)
                                SELECT -1
                            ELSE
                                BEGIN
                                    INSERT INTO Customers(CustomerName, ContactName, Province, Address, Phone, Email, IsLocked)
                                    VALUES(@CustomerName, @ContactName, @Province, @Address, @Phone, @Email, @IsLocked);
                                    SELECT @@IDENTITY;
                                END";
				var parameters = new
				{
					dto.CustomerName,
					dto.ContactName,
					dto.Province,
					dto.Address,
					dto.Phone,
					dto.Email,
					dto.IsLocked
				};
				id = await connection.ExecuteScalarAsync<int>(sql: sql, param: parameters, commandType: CommandType.Text);
			}
			return id;
		}

		public async Task<bool> DeleteCustomer(int id)
		{
			bool result;
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var sql = @"DELETE FROM Customers 
                            WHERE CustomerId = @customerId AND NOT EXISTS
                            (SELECT CustomerId FROM Orders WHERE CustomerId = @customerId)";
				result = await connection.ExecuteAsync(sql: sql, param: new { customerId = id }, commandType: CommandType.Text) > 0;
			}
			return result;
		}

		public async Task<Customer?> GetCustomerById(int id)
		{
			Customer? customer;
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var sql = "SELECT * FROM Customers WHERE CustomerId = @customerId";
				customer = await connection.QueryFirstOrDefaultAsync<Customer>(sql: sql, param: new { customerId = id }, commandType: CommandType.Text);
			}
			return customer;
		}

		public async Task<IReadOnlyList<Customer>> LoadCustomers(CustomerFilterDto filter)
		{
			var customers = new List<Customer>();

			string searchValue = filter.SearchValue ?? string.Empty;
			if (!string.IsNullOrEmpty(searchValue))
			{
				searchValue = $"%{searchValue}%";
			}

			using (var connection = ConnectDB.LiteCommerceDB())
			{
				const string sql = @"
	WITH cte AS (
		SELECT 
			CustomerID,
			CustomerName,
			ContactName,
			Province,
			Address,
			Phone,
			Email,
			Password,
			IsLocked,
			ROW_NUMBER() OVER (ORDER BY CustomerName) AS RowNumber
		FROM Customers
		WHERE (@searchValue = '') OR (CustomerName LIKE @searchValue)
	)
	SELECT 
		CustomerID,
		CustomerName,
		ContactName,
		Province,
		Address,
		Phone,
		Email,
		Password,
		IsLocked
	FROM cte
	WHERE (@pageSize = 0)
		OR (RowNumber BETWEEN (@page - 1) * @pageSize + 1 AND @page * @pageSize)
	ORDER BY RowNumber;";

				var parameters = new
				{
					page = filter.Page,
					pageSize = filter.PageSize,
					searchValue
				};

				var result = await connection.QueryAsync<Customer>(sql, parameters, commandType: CommandType.Text);
				customers = result?.ToList() ?? new List<Customer>();
			}

			return customers;
		}


		public async Task<bool> UpdateCustomer(EditCustomerDto dto)
		{
			bool result;
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var sql = @"IF NOT EXISTS (SELECT * FROM Customers WHERE CustomerId <> @CustomerId AND Email = @Email)
                            BEGIN
                                UPDATE Customers SET
                                    CustomerName = @CustomerName,
                                    ContactName = @ContactName,
                                    Province = @Province,
                                    Address = @Address,
                                    Phone = @Phone,
                                    Email = @Email,
                                    IsLocked = @IsLocked
                                WHERE CustomerId = @CustomerId
                            END";
				var parameters = new
				{
					dto.CustomerId,
					dto.CustomerName,
					dto.ContactName,
					dto.Province,
					dto.Address,
					dto.Phone,
					dto.Email,
					dto.IsLocked
				};
				result = await connection.ExecuteAsync(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
			}
			return result;
		}
	}
}
