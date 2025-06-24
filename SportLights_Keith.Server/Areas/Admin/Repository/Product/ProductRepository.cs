using Dapper;
using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Products;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Products.Abstractions;
using SPORTLIGHTS_SERVER.Entities;
using SPORTLIGHTS_SERVER.Modules;
using System.Data;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Repository.Products
{
	public class ProductRepository : IProductRepository
	{
		public async Task<int> CountProducts(ProductFilterDto filter, int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
		{
			if (!string.IsNullOrEmpty(filter.SearchValue))
				filter.SearchValue = "%" + filter.SearchValue + "%";

			using var connection = ConnectDB.LiteCommerceDB();
			var sql = @"select count(*) from Products
		where (@searchValue = N'' or ProductName like @searchValue)
		and (@supplierID = 0 or SupplierID = @supplierID)
		and (@categoryID = 0 or CategoryID = @categoryID)
		and ((@minPrice = 0 and @maxPrice = 0) or (ProductName between @minPrice and @maxPrice))";

			var parameters = new
			{
				searchValue = filter.SearchValue,
				categoryID,
				supplierID,
				minPrice,
				maxPrice
			};

			return await connection.ExecuteScalarAsync<int>(sql, parameters);
		}

		public async Task<int> CreateProduct(CreateProductDto dto)
		{
			using var connection = ConnectDB.LiteCommerceDB();

			var sql = @"
		if exists(select * from Products where ProductName =  @ProductName)
			select -1
		else
		begin
			INSERT INTO Products(ProductName, CategoryID, SupplierID, Unit, Price, Photo)
			VALUES(@ProductName, @CategoryID, @SupplierID, @Unit, @Price, @Photo);
			SELECT CAST(SCOPE_IDENTITY() as int);
		end";

			var parameters = new
			{
				//Productid = dto.ProductId,
				ProductName = dto.ProductName ?? "",
				CategoryID = dto.CategoryId,
				SupplierID = dto.SupplierId,
				Unit = dto.Unit,
				Price = dto.Price,
				Photo = dto.Photo ?? "",
			};

			return await connection.ExecuteScalarAsync<int>(sql, parameters);
		}

		public async Task<bool> DeleteProduct(int productId)
		{
			using var connection = ConnectDB.LiteCommerceDB();

			var sql = @"
		DELETE FROM Products 
		WHERE ProductID = @productID 
		AND NOT EXISTS (SELECT * FROM OrderDetails WHERE ProductID = @productID);

		DELETE FROM ProductPhotos WHERE ProductID = @productID;
		DELETE FROM ProductAttributes WHERE ProductID = @productID;";

			var parameters = new { productID = productId };

			var affectedRows = await connection.ExecuteAsync(sql, parameters);
			return affectedRows > 0;
		}

		public async Task<Product?> GetProductById(int productId)
		{
			using var connection = ConnectDB.LiteCommerceDB();

			var sql = "SELECT * FROM Products WHERE ProductID = @productID";
			var parameters = new { productID = productId };

			return await connection.QueryFirstOrDefaultAsync<Product>(sql, parameters);
		}

		public async Task<IReadOnlyList<Product>> GetProducts(ProductFilterDto filter)
		{
			using var connection = ConnectDB.LiteCommerceDB();

			var sql = @"
		WITH cte AS
		(
			SELECT *,
			ROW_NUMBER() OVER (ORDER BY ProductName) AS RowNumber
			FROM Products
			WHERE (@searchValue = N'' OR ProductName LIKE @searchValue)
			AND (@supplierID = 0 OR SupplierID = @supplierID)
			AND (@categoryID = 0 OR CategoryID = @categoryID)
			AND ((@minPrice = 0 AND @maxPrice = 0) OR (ProductName BETWEEN @minPrice AND @maxPrice))
		)
		SELECT * FROM cte
		WHERE (@pageSize = 0)
		OR (RowNumber BETWEEN (@page - 1) * @pageSize + 1 AND @page * @pageSize)
		ORDER BY RowNumber;";

			var parameters = new
			{
				page = filter.Page,
				pageSize = filter.PageSize,
				searchValue = filter.SearchValue,
				categoryID = 0,
				supplierID = 0,
				minPrice = 0,
				maxPrice = 0
			};

			var result = await connection.QueryAsync<Product>(sql, parameters);
			return result.ToList();
		}

		public async Task<bool> UpdateProduct(EditProductDto data)
		{
			using var connection = ConnectDB.LiteCommerceDB();

			var sql = @"
		IF NOT EXISTS (SELECT * FROM Products WHERE ProductName = @productName AND ProductID <> @productID)
		BEGIN
			UPDATE Products 
			SET ProductName = @productName,
				ProductDescription = @productDescription,
				SupplierID = @supplierID,
				CategoryID = @categoryID,
				Unit = @unit,
				Price = @price,
				Photo = @photo,
				IsSelling = @isSelling
			WHERE ProductID = @productID
		END";

			var parameters = new
			{
				productID = data.ProductId,
				productName = data.ProductName ?? "",
				productDescription = data.ProductDescription ?? "",
				supplierID = data.SupplierId,
				categoryID = data.CategoryId,
				unit = data.Unit ?? "",
				price = data.Price,
				photo = data.Photo ?? "",
				isSelling = data.IsSelling
			};

			var affected = await connection.ExecuteAsync(sql, parameters);
			return affected > 0;
		}

	}
}
