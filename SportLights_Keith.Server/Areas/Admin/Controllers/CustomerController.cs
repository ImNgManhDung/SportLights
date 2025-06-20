using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Categories;
using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Customers;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.CustomerRepository;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Customers.Abstractions;
using SPORTLIGHTS_SERVER.Authen.Helpers;
using SPORTLIGHTS_SERVER.Constants;
using SPORTLIGHTS_SERVER.Entities;
using SPORTLIGHTS_SERVER.Modules;
using System.Reflection;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Controllers
{
	[Authorize(Roles = WebUserRoles.Administrator)]
	[Route("api/v1/admin")]
	[ApiController]
	public class CustomerController : ControllerBase
	{
		#region Repository
		private readonly ICustomerRepository _customerRepo = new CustomerRepository();
		private readonly RedisCacheService _cache;

		public CustomerController(RedisCacheService cache)
		{
			_cache = cache;
		}

		#endregion

		private const int PAGE_SIZE = 10;
		private const string MsgCustomerExists = "Customer already exists";
		private const string MsgCustomerNameRequired = "Customer name is required";
		private const string MsgCustomerNotFound = "Customer not found";
		private const string MsgError = "An error has occurred";
		private const string MsgSuccess = "Success";

		[HttpGet("customer")]
		public async Task<IActionResult> GetCustomers([FromQuery] CustomerFilterDto viewData)
		{		
			viewData = new CustomerFilterDto()
			{
				SearchValue = viewData.SearchValue,
				Page = viewData.Page,
				PageSize = PAGE_SIZE,
			};
			string cacheKey = CacheKeyHelper.Customer(viewData.SearchValue, viewData.Page);

			var cachedCustomers = await _cache.GetCacheAsync<PaginatedCustomerDto>(cacheKey);
			if (cachedCustomers != null)
			{
				return Ok(new
				{
					response_code = ResponseCodes.Success,
					results = cachedCustomers,
					source = "cache"
				});
			}

			var result = new PaginatedCustomerDto
			{
				SearchValue = viewData.SearchValue,
				CurrentPage = viewData.Page,
				CurrentPageSize = viewData.PageSize,
				TotalRow = _customerRepo.Count(viewData),
				Data = await _customerRepo.LoadCustomers(viewData)
			};

			await _cache.SetCacheAsync(cacheKey, result, TimeSpan.FromMinutes(5));

			return Ok(new
			{
				response_code = ResponseCodes.Success,
				results = result,
				source = "db"
			});
		}

			[HttpGet("customer/{id}")]
			public async Task<IActionResult> GetCustomer(int customerid)
			{
				var customer = await _customerRepo.GetCustomerById(customerid);
				if (customer == null)
				{
					return NotFound(MsgCustomerNotFound);
				}

				return Ok(new
				{
					response_code = ResponseCodes.Success,
					results = customer
				});
			}

		[HttpPost("customer")]
		public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto customer)
		{
			if (customer == null || string.IsNullOrWhiteSpace(customer.CustomerName))
				return BadRequest(MsgCustomerNameRequired);

			var newId = await _customerRepo.CreateCustomer(customer);
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
		public async Task<IActionResult> UpdateCustomer(int customerid, [FromBody] EditCustomerDto customer)
		{
			if (customer == null || customerid != customer.CustomerId)
				return BadRequest(MsgError);

			var existingCustomer = await _customerRepo.GetCustomerById(customerid);
			if (existingCustomer == null)
				return NotFound(MsgCustomerNotFound);

			var isUpdated = await _customerRepo.UpdateCustomer(customer);
			if (!isUpdated)
				return StatusCode(500, MsgError);

			return Ok(new
			{
				response_code = ResponseCodes.NoContent,
				customer_id = customerid,
				results = MsgSuccess
			});
		}

		[HttpDelete("customer/{customerId}")]
		public async Task<IActionResult> DeleteCustomer(int customerId)
		{
			if (customerId <= 0)
				return BadRequest(MsgCustomerNotFound);

			var Isdeleted = await _customerRepo.DeleteCustomer(customerId);
			if (!Isdeleted)
				return BadRequest(MsgCustomerNotFound);

			return Ok(new
			{
				response_code = ResponseCodes.NoContent,
				customer_id = customerId,
				results = MsgSuccess
			});
		}
	}
}

