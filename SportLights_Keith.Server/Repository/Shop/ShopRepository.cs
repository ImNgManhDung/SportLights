using Dapper;
using SPORTLIGHTS_SERVER.Entities;
using SPORTLIGHTS_SERVER.Models.Shop;
using SPORTLIGHTS_SERVER.Modules;
using SPORTLIGHTS_SERVER.Repository.Shop.Abstractions;

namespace SPORTLIGHTS_SERVER.Repository.Shop
{
	public class ShopRepository : IShopRepository
	{

		public IReadOnlyList<Product> GetProducts(ViewFilterShop viewData)
		{
			using (var conn = ConnectDB.LiteCommerceDB())
			{
				var sqlGetProducts = $@"select * from Products ";
				var parameters = new
				{
					SearchValue = $"%{viewData.SearchValue}%",
				};
				var commandGetProducts = new CommandDefinition(sqlGetProducts, parameters: parameters, flags: CommandFlags.NoCache);

				var data = conn.Query<Product>(commandGetProducts)
					.Distinct()
					.ToList();

				return data;
			}
		}
	}
}
