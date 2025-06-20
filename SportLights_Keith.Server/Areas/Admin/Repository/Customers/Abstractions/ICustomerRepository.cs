using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Customers;
using SPORTLIGHTS_SERVER.Entities;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Repository.Customers.Abstractions
{
	public interface ICustomerRepository
	{
		Task<IReadOnlyList<Customer>> LoadCustomers(CustomerFilterDto filter);

		int Count(CustomerFilterDto filter);

		Task<Customer?> GetCustomerById(int id);

		Task<int> CreateCustomer(CreateCustomerDto customer);

		Task<bool> UpdateCustomer(EditCustomerDto customer);

		Task<bool> DeleteCustomer(int id);
	}
}
