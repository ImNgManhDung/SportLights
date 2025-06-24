using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Products;
using SPORTLIGHTS_SERVER.Entities;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Repository.Products.Abstractions
{
	public interface IProductRepository
	{
		Task<int> CountProducts(ProductFilterDto filter, int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0);

		Task<int> CreateProduct(CreateProductDto dto);

		Task<bool> DeleteProduct(int productId);

		Task<Product?> GetProductById(int productId);

		Task<IReadOnlyList<Product>> GetProducts(ProductFilterDto filter);

		Task<bool> UpdateProduct(EditProductDto data);
	}
}
