using Dapper;
using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Suppliers;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Suppliers.Abstractions;
using SPORTLIGHTS_SERVER.Entities;
using SPORTLIGHTS_SERVER.Modules;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Repository.Suppliers
{
	public class SupplierRepository : ISupplierRepository
	{

		public int Count(SupplierFilterDto filter)
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

				return connection.ExecuteScalar<int>(sql, parameters);
			}
		}

		public int CreateSupplier(CreateSupplierDto dto)
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

				return connection.ExecuteScalar<int>(sql, parameters);
			}
		}

		public bool DeleteSupplier(int id)
		{
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				string sql = @"DELETE FROM Suppliers
                               WHERE SupplierId = @SupplierId
                               AND NOT EXISTS (SELECT * FROM Products WHERE SupplierId = @SupplierId)";

				return connection.Execute(sql, new { SupplierId = id }) > 0;
			}
		}

		public Supplier? GetSupplierById(int id)
		{
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				string sql = "SELECT * FROM Suppliers WHERE SupplierId = @SupplierId";
				return connection.QueryFirstOrDefault<Supplier>(sql, new { SupplierId = id });
			}
		}

		public List<Supplier> GetSuppliers(SupplierFilterDto filter)
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

				return connection.Query<Supplier>(sql, parameters).ToList();
			}
		}

		public bool UpdateSupplier(EditSupplierDto dto)
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

				return connection.Execute(sql, parameters) > 0;
			}
		}
	}
}