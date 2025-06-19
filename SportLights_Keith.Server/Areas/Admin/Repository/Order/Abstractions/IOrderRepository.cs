using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Orders;
using SPORTLIGHTS_SERVER.Entities;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Repository.Orders.Abstractions
{
	public interface IOrderRepository
	{
		List<Order> GetOrders(OrderFilterDto filter);
		Order? GetOrderById(int orderId);
		int CreateOrder(CreateOrderDto dto);
		bool UpdateOrder(EditOrderDto dto);
		bool DeleteOrder(int orderId);
		int Count(OrderFilterDto filter);
	}
}