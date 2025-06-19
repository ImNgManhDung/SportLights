using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Suppliers;
using SPORTLIGHTS_SERVER.Entities;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Repository.Suppliers.Abstractions
{
	public interface ISupplierRepository
	{
		List<Supplier> GetSuppliers(SupplierFilterDto filter);
		Supplier? GetSupplierById(int id);
		int CreateSupplier(CreateSupplierDto dto);
		bool UpdateSupplier(EditSupplierDto dto);
		bool DeleteSupplier(int id);
		int Count(SupplierFilterDto filter);
	}
}
