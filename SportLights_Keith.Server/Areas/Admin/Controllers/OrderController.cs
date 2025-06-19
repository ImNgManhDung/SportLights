using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Orders;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.OrderRepository;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Orders.Abstractions;
using SPORTLIGHTS_SERVER.Authen.Helpers;
using SPORTLIGHTS_SERVER.Constants;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Controllers
{
	//[Authorize(Roles = WebUserRoles.Administrator)]
	[Route("api/v1/admin/order")]
	[ApiController]
	public class OrderController : ControllerBase
	{
		private readonly IOrderRepository _orderRepo = new OrderRepository();

		private const int PAGE_SIZE = 10;
		private const string MsgOrderNotFound = "Đơn hàng không tồn tại";
		private const string MsgError = "Có lỗi xảy ra";
		private const string MsgSuccess = "Thành công";

		[HttpGet]
		public IActionResult GetOrders([FromQuery] OrderFilterDto filter)
		{
			filter.PageSize = PAGE_SIZE;

			var result = new PaginatedOrderDto
			{
				SearchValue = filter.SearchValue,
				CurrentPage = filter.Page,
				CurrentPageSize = filter.PageSize,
				TotalRow = _orderRepo.Count(filter),
				Data = _orderRepo.GetOrders(filter)
			};

			return Ok(new
			{
				response_code = ResponseCodes.Success,
				results = result
			});
		}

		[HttpGet("{id}")]
		public IActionResult GetOrder(int id)
		{
			var order = _orderRepo.GetOrderById(id);
			if (order == null)
				return NotFound(MsgOrderNotFound);

			return Ok(order);
		}

		[HttpPost]
		public IActionResult CreateOrder([FromBody] CreateOrderDto dto)
		{
			var newId = _orderRepo.CreateOrder(dto);
			if (newId <= 0)
				return StatusCode(500, MsgError);

			return Ok(new
			{
				response_code = ResponseCodes.Created,
				order_id = newId,
				results = MsgSuccess
			});
		}

		[HttpPut("{id}")]
		public IActionResult UpdateOrder(int id, [FromBody] EditOrderDto dto)
		{
			if (id != dto.OrderId)
				return BadRequest("ID không khớp");

			var existing = _orderRepo.GetOrderById(id);
			if (existing == null)
				return NotFound(MsgOrderNotFound);

			var success = _orderRepo.UpdateOrder(dto);
			if (!success)
			{
                return StatusCode(500, MsgError);
			}

			return Ok(MsgSuccess);
		}

		[HttpDelete("{id}")]
		public IActionResult DeleteOrder(int id)
		{
			if (id <= 0)
				return BadRequest(MsgOrderNotFound);

			var deleted = _orderRepo.DeleteOrder(id);
			if (!deleted)
				return BadRequest(MsgOrderNotFound);

			return Ok(MsgSuccess);
		}
	}
}
