using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Shippers;
using SPORTLIGHTS_SERVER.Entities;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Repository.Shippers.Abstractions
{
	public interface IShipperRepository
	{
		Task<int> Count(ShipperFilterDto filter);

		Task<int> CreateShipper(CreateShipperDto dto);

		Task<bool> DeleteShipper(int id);

		Task<Shipper?> GetShipperById(int id);

		Task<List<Shipper>> GetShippers(ShipperFilterDto filter);

		Task<bool> UpdateShipper(EditShipperDto dto);
	}
}
