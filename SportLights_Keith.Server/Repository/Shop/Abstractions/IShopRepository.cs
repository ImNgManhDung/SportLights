using SPORTLIGHTS_SERVER.Entities;
using SPORTLIGHTS_SERVER.Models.Shop;

namespace SPORTLIGHTS_SERVER.Repository.Shop.Abstractions
{
	public interface IShopRepository
	{
		IReadOnlyList<Product> GetProducts(ViewFilterShop viewData);
	}
}
