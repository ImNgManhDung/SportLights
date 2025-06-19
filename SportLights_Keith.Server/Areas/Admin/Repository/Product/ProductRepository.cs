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
		public int CountProducts(ProductFilterDto filter, int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
		{
			int count = 0;
			if (!string.IsNullOrEmpty(filter.SearchValue))
				filter.SearchValue = "%" + filter.SearchValue + "%";

			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var sql = @"select count(*) from Products
                    where (@searchValue = N'' or ProductName like @searchValue)
                    and (@supplierID = 0 or SupplierID = @supplierID)
                    and (@categoryID = 0 or CategoryID = @categoryID)
                    and ((@minPrice = 0 and @maxPrice = 0) or (ProductName between @minPrice and @maxPrice))";

				var parameters = new
				{
					searchValue = filter.SearchValue,
					categoryID = categoryID,
					supplierID = supplierID,
					minPrice = minPrice,
					maxPrice = maxPrice
				};

				count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
				connection.Close();
			}

			return count;
		}

		public int CreateProduct(CreateProductDto dto)
		{
			int id = 0;
			using (var connection = ConnectDB.LiteCommerceDB())
			{

				var sql = @"	if exists(select * from Products where Productid =  @Productid)
                         select -1
                     else
                         begin
                            INSERT INTO Products(ProductName, CategoryID, SupplierID, Unit, Price, Photo)
                             VALUES(@ProductName, @CategoryID, @SupplierID, @Unit, @Price, @Photo);
                            SELECT @@identity;
                         end";
				var parameters = new
				{
					Productid = dto.ProductId,
					ProductName = dto.ProductName ?? "",
					CategoryID = dto.CategoryId,
					SupplierID = dto.SupplierId,
					Unit = dto.Unit,
					Price = dto.Price,
					Photo = dto.Photo ?? "",

				};
				id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
				connection.Close();
			};
			return id;
		}

		public bool DeleteProduct(int productId)
		{
			bool result = false;
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var sql = @"
					DELETE FROM Products 
					WHERE ProductID = @productID 
					AND NOT EXISTS (SELECT * FROM OrderDetails WHERE ProductID = @productID);

					DELETE FROM ProductPhotos 
					WHERE ProductID = @productID;

					DELETE FROM ProductAttributes 
					WHERE ProductID = @productID;";

				var parameters = new { productID = productId };
				result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
			}
			return result;
		}

		public Product? GetProductById(int productId)
		{
			Product? data = null;
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var sql = "select * from Products where ProductID = @productID";
				var parameters = new { productID = productId };
				data = connection.QueryFirstOrDefault<Product>(sql: sql, param: parameters, commandType: CommandType.Text);
				connection.Close();
			}
			return data;
		}

		public IReadOnlyList<Product> GetProducts(ProductFilterDto filter)
		{
			List<Product> data;
			using (var connection = ConnectDB.LiteCommerceDB())
			{

				var sql = @"with cte as
                (
                    select *,
                    ROW_NUMBER() over (order by ProductName) as RowNumber
                    from Products
                    where (@searchValue = N'' or ProductName like @searchValue)
                    and (@supplierID = 0 or SupplierID = @supplierID)
                    and (@categoryID = 0 or CategoryID = @categoryID)
                    and ((@minPrice = 0 and @maxPrice = 0) or (ProductName between @minPrice and @maxPrice))
                )

                select * from cte
                where (@pageSize = 0)
                    or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                    order by RowNumber;";

				var parameters = new
				{
					page = filter.Page,
					pageSize = filter.PageSize,
					searchValue = filter.SearchValue,
					categoryID = 0,
					supplierID = 0,
					minPrice = 0,
					maxPrice = 0,
				};

				data = connection.Query<Product>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
				connection.Close();
				return data;
			}
		}

		public bool UpdateProduct(EditProductDto data)
		{
			bool result = false;
			using (var connection = ConnectDB.LiteCommerceDB())
			{
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

				result = connection.Execute(sql, parameters, commandType: CommandType.Text) > 0;
			}
			return result;
		}
	}
}
