using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Customers;
using SPORTLIGHTS_SERVER.Entities;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Repository.Customers.Abstractions
{
	public interface ICustomerRepository
	{
		// Lấy danh sách khách hàng có phân trang + filter
		List<Customer> GetCustomers(CustomerFilterDto filter);

		// Đếm tổng số khách hàng (phục vụ phân trang)
		int Count(CustomerFilterDto filter);

		// Lấy chi tiết 1 khách hàng
		Customer? GetCustomerById(int id);

		// Tạo khách hàng mới
		int CreateCustomer(CreateCustomerDto customer);

		// Cập nhật thông tin khách hàng
		bool UpdateCustomer(EditCustomerDto customer);

		// Xóa khách hàng
		bool DeleteCustomer(int id);
	}
}
