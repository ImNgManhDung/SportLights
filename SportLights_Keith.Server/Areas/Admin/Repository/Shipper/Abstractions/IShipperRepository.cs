using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Shippers;
using SPORTLIGHTS_SERVER.Entities;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Repository.Shippers.Abstractions
{
	public interface IShipperRepository
	{
		List<Shipper> GetShippers(ShipperFilterDto filter);
		Shipper? GetShipperById(int id);
		int CreateShipper(CreateShipperDto dto);
		bool UpdateShipper(EditShipperDto dto);
		bool DeleteShipper(int id);
		int Count(ShipperFilterDto filter);
	}
}
