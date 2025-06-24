using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Orders;
using SPORTLIGHTS_SERVER.Entities;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Repository.Orders.Abstractions
{
	public interface IOrderRepository
	{
		Task<List<Order>> LoadOrders(OrderFilterDto filter);

		Task<Order?> GetOrderById(int orderId);

		Task<int> CreateOrder(CreateOrderDto dto);

		Task<bool> UpdateOrder(EditOrderDto dto);

		Task<bool> DeleteOrder(int orderId);

		Task<int> Count(OrderFilterDto filter);
	}
}