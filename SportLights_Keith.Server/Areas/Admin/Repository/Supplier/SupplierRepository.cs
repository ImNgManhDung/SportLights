using Dapper;
using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Suppliers;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Suppliers.Abstractions;
using SPORTLIGHTS_SERVER.Entities;
using SPORTLIGHTS_SERVER.Modules;
using System.Data;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Repository.Suppliers
{
	public class SupplierRepository : ISupplierRepository
	{
		public async Task<bool> CheckCreateSupplier(string supplierName)
		{
			using (var conn = ConnectDB.LiteCommerceDB())
			{
				var sqlCheckCreateSupplier = $@"SELECT SupplierName
		                  FROM Suppliers
		                  WHERE SupplierName = @SupplierName";

				var parameters = new
				{
					SupplierName = supplierName,
				};

				var command = new CommandDefinition(sqlCheckCreateSupplier, parameters: parameters, flags: CommandFlags.NoCache);

				var supplierInfo = await conn.QueryFirstOrDefaultAsync<Supplier>(command);

				return supplierInfo != null;
			}
		}

		public async Task<int> Count(SupplierFilterDto filter)
		{
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				string sql = @"SELECT COUNT(*) FROM Suppliers
                               WHERE (@SearchValue = N'' OR SupplierName LIKE @SearchPattern)";

				var parameters = new
				{
					SearchValue = filter.SearchValue ?? string.Empty,
					SearchPattern = $"%{filter.SearchValue}%"
				};

				return await connection.ExecuteScalarAsync<int>(sql, parameters);
			}
		}

		public async Task<int> CreateSupplier(CreateSupplierDto dto)
		{
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				string sql = @"IF EXISTS (SELECT * FROM Suppliers WHERE Email = @Email)
                                 SELECT -1
                              ELSE
                              BEGIN
                                  INSERT INTO Suppliers (SupplierName, ContactName, Provice, Address, Phone, Email)
                                  VALUES (@SupplierName, @ContactName, @Provice, @Address, @Phone, @Email);
                                  SELECT CAST(SCOPE_IDENTITY() AS INT);
                              END";

				var parameters = new
				{
					dto.SupplierName,
					dto.ContactName,
					dto.Provice,
					dto.Address,
					dto.Phone,
					dto.Email
				};

				return await connection.ExecuteScalarAsync<int>(sql, parameters);
			}
		}

		public async Task<bool> DeleteSupplier(int id)
		{
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				string sql = @"DELETE FROM Suppliers
                               WHERE SupplierId = @SupplierId
                               AND NOT EXISTS (SELECT * FROM Products WHERE SupplierId = @SupplierId)";

				return await connection.ExecuteAsync(sql, new { SupplierId = id }) > 0;
			}
		}

		public async Task<Supplier?> GetSupplierById(int id)
		{
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				string sql = "SELECT SupplierId, SupplierName, ContactName, Provice, Address, Phone, Email FROM Suppliers WHERE SupplierId = @SupplierId";
				return await connection.QueryFirstOrDefaultAsync<Supplier>(sql, new { SupplierId = id });
			}
		}

		public async Task<IReadOnlyList<Supplier>> LoadSuppliers(SupplierFilterDto filter)
		{
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				string sql = @"WITH cte AS
                              (
                                  SELECT SupplierId, SupplierName, ContactName, Provice, Address, Phone, Email,
                                         ROW_NUMBER() OVER (ORDER BY SupplierName) AS RowNumber
                                  FROM Suppliers
                                  WHERE (@SearchValue = N'' OR SupplierName LIKE @SearchPattern)
                              )
                              SELECT * FROM cte
                              WHERE (@PageSize = 0)
                              OR (RowNumber BETWEEN (@Page - 1) * @PageSize + 1 AND @Page * @PageSize)
                              ORDER BY RowNumber";

				var parameters = new
				{
					Page = filter.Page,
					PageSize = filter.PageSize,
					SearchValue = filter.SearchValue ?? string.Empty,
					SearchPattern = $"%{filter.SearchValue}%"
				};

				var result = await connection.QueryAsync<Supplier>(sql, parameters);
				return result.ToList();
			}
		}

		public async Task<bool> UpdateSupplier(EditSupplierDto dto)
		{
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				string sql = @"IF NOT EXISTS (SELECT * FROM Suppliers WHERE SupplierId <> @SupplierId AND Email = @Email)
                              BEGIN
                                  UPDATE Suppliers
                                  SET SupplierName = @SupplierName,
                                      ContactName = @ContactName,
                                      Provice = @Provice,
                                      Address = @Address,
                                      Phone = @Phone,
                                      Email = @Email
                                  WHERE SupplierId = @SupplierId
                              END";

				var parameters = new
				{
					dto.SupplierId,
					dto.SupplierName,
					dto.ContactName,
					dto.Provice,
					dto.Address,
					dto.Phone,
					dto.Email
				};

				return await connection.ExecuteAsync(sql, parameters) > 0;
			}
		}
		
	}
}
