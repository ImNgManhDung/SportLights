using Dapper;
using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Shippers;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Shippers.Abstractions;
using SPORTLIGHTS_SERVER.Entities;
using SPORTLIGHTS_SERVER.Modules;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Repository.Shippers
{
	public class ShipperRepository : IShipperRepository
	{
		public int Count(ShipperFilterDto filter)
		{
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var searchValue = string.IsNullOrWhiteSpace(filter.SearchValue)
					? ""
					: $"%{filter.SearchValue}%";

				var sql = @"SELECT COUNT(*) FROM Shippers
                            WHERE (@searchValue = N'') OR (ShipperName LIKE @searchValue)";
				return connection.ExecuteScalar<int>(sql, new { searchValue });
			}
		}

		public int CreateShipper(CreateShipperDto dto)
		{
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var sql = @"IF EXISTS(SELECT * FROM Shippers WHERE ShipperName = @ShipperName OR Phone = @Phone)
                                SELECT -1
                            ELSE
                            BEGIN
                                INSERT INTO Shippers(ShipperName, Phone)
                                VALUES(@ShipperName, @Phone);
                                SELECT CAST(SCOPE_IDENTITY() AS INT);
                            END";
				var parameters = new
				{
					ShipperName = dto.ShipperName?.Trim() ?? "",
					Phone = dto.Phone?.Trim() ?? ""
				};
				return connection.ExecuteScalar<int>(sql, parameters);
			}
		}

		public bool DeleteShipper(int id)
		{
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var sql = @"DELETE FROM Shippers 
                            WHERE ShipperID = @id 
                            AND NOT EXISTS(SELECT * FROM Orders WHERE ShipperID = @id)";
				return connection.Execute(sql, new { id }) > 0;
			}
		}

		public Shipper? GetShipperById(int id)
		{
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var sql = "SELECT * FROM Shippers WHERE ShipperID = @id";
				return connection.QueryFirstOrDefault<Shipper>(sql, new { id });
			}
		}

		public List<Shipper> GetShippers(ShipperFilterDto filter)
		{
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var searchValue = string.IsNullOrWhiteSpace(filter.SearchValue)
					? ""
					: $"%{filter.SearchValue}%";

				var sql = @"
                    WITH cte AS (
                        SELECT ShipperID, ShipperName, Phone,
                            ROW_NUMBER() OVER (ORDER BY ShipperName) AS RowNumber
                        FROM Shippers
                        WHERE (@searchValue = N'') OR (ShipperName LIKE @searchValue)
                    )
                    SELECT * FROM cte
                    WHERE (@PageSize = 0)
                       OR (RowNumber BETWEEN (@Page - 1) * @PageSize + 1 AND @Page * @PageSize)
                    ORDER BY RowNumber";
				var parameters = new
				{
					Page = filter.Page,
					PageSize = filter.PageSize,
					searchValue
				};
				return connection.Query<Shipper>(sql, parameters).ToList();
			}
		}

		public bool UpdateShipper(EditShipperDto dto)
		{
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var sql = @"
                    IF NOT EXISTS (
                        SELECT * FROM Shippers 
                        WHERE ShipperID <> @ShipperID 
                          AND (ShipperName = @ShipperName OR Phone = @Phone)
                    )
                    BEGIN
                        UPDATE Shippers
                        SET ShipperName = @ShipperName,
                            Phone = @Phone
                        WHERE ShipperID = @ShipperID
                    END";
				var parameters = new
				{
					ShipperID = dto.ShipperId,
					ShipperName = dto.ShipperName?.Trim() ?? "",
					Phone = dto.Phone?.Trim() ?? ""
				};
				return connection.Execute(sql, parameters) > 0;
			}
		}
	}
}
