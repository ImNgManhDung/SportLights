using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Customers;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.CustomerRepository;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Customers.Abstractions;
using SPORTLIGHTS_SERVER.Authen.Helpers;
using SPORTLIGHTS_SERVER.Constants;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Controllers
{
	//[Authorize(Roles = WebUserRoles.Administrator)]
	[Route("api/v1/admin")]
	[ApiController]
	public class CustomerController : ControllerBase
	{
		#region Repository
		private readonly ICustomerRepository _customerRepo = new CustomerRepository();

		#endregion


		private const int PAGE_SIZE = 10;
		private const string MsgCustomerExists = "Khách hàng đã tồn tại";
		private const string MsgCustomerNameRequired = "Tên khách hàng không được để trống";
		private const string MsgCustomerNotFound = "Khách hàng không tồn tại";
		private const string MsgError = "Có lỗi xảy ra";
		private const string MsgSuccess = "Thành công";

		[HttpGet("customer")]
		public IActionResult GetCustomers([FromQuery] CustomerFilterDto filter)
		{
			filter.PageSize = PAGE_SIZE;

			var result = new PaginatedCustomerDto
			{
				SearchValue = filter.SearchValue,
				CurrentPage = filter.Page,
				CurrentPageSize = filter.PageSize,
				TotalRow = _customerRepo.Count(filter),
				Data = _customerRepo.GetCustomers(filter)
			};

			return Ok(new
			{
				response_code = ResponseCodes.Success,
				results = result
			});
		}

		[HttpGet("customer/{id}")]
		public IActionResult GetCustomer(int id)
		{
			var customer = _customerRepo.GetCustomerById(id);
			if (customer == null)
				return NotFound(MsgCustomerNotFound);

			return Ok(customer);
		}

		[HttpPost("customer")]
		public IActionResult CreateCustomer([FromBody] CreateCustomerDto customer)
		{
			if (customer == null || string.IsNullOrWhiteSpace(customer.CustomerName))
				return BadRequest(MsgCustomerNameRequired);

			var newId = _customerRepo.CreateCustomer(customer);
			if (newId <= 0)
				return StatusCode(500, MsgError);

			return Ok(new
			{
				response_code = ResponseCodes.Created,
				customer_id = newId,
				results = MsgSuccess
			});
		}

		[HttpPut("customer/{customerid}")]
		public IActionResult UpdateCustomer(int customerid, [FromBody] EditCustomerDto customer)
		{
			if (customer == null || customerid != customer.CustomerId)
				return BadRequest(MsgError);

			var exists = _customerRepo.GetCustomerById(customerid);
			if (exists == null)
				return NotFound(MsgCustomerNotFound);

			var updated = _customerRepo.UpdateCustomer(customer);
			if (!updated)
				return StatusCode(500, MsgError);

			return Ok(MsgSuccess);
		}

		[HttpDelete("customer/{id}")]
		public IActionResult DeleteCustomer(int id)
		{
			if (id <= 0)
				return BadRequest(MsgCustomerNotFound);

			var deleted = _customerRepo.DeleteCustomer(id);
			if (!deleted)
				return BadRequest(MsgCustomerNotFound);

			return Ok(MsgSuccess);
		}
	}
}

