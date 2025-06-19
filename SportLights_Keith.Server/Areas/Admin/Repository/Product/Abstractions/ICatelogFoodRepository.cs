using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Products;
using SPORTLIGHTS_SERVER.Entities;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Repository.Products.Abstractions
{
	public interface IProductRepository
	{
		int CountProducts(ProductFilterDto filter, int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0);

		IReadOnlyList<Product> GetProducts(ProductFilterDto filter);

		Product? GetProductById(int productId);

		int CreateProduct(CreateProductDto dto);

		bool UpdateProduct(EditProductDto dto);

		bool DeleteProduct(int id);
	}
}
