using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Suppliers;
using SPORTLIGHTS_SERVER.Entities;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Repository.Suppliers.Abstractions
{
	public interface ISupplierRepository
	{
		Task<bool> CheckCreateSupplier(string supplierName);
		Task<int> Count(SupplierFilterDto filter);
		Task<IReadOnlyList<Supplier>> LoadSuppliers(SupplierFilterDto filter);
		Task<Supplier?> GetSupplierById(int id);
		Task<int> CreateSupplier(CreateSupplierDto dto);
		Task<bool> UpdateSupplier(EditSupplierDto dto);
		Task<bool> DeleteSupplier(int id);



	}
}
