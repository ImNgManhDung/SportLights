using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Categories;
using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Suppliers;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Suppliers;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Suppliers.Abstractions;
using SPORTLIGHTS_SERVER.Authen.Helpers;
using SPORTLIGHTS_SERVER.Constants;
using SPORTLIGHTS_SERVER.Modules;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Controllers
{
	// [Authorize(Roles = WebUserRoles.Administrator)]
	[Route("api/v1/admin")]
	[ApiController]
	public class SupplierController : ControllerBase
	{
		private readonly ISupplierRepository _supplierRepo = new SupplierRepository();
		private readonly RedisCacheService _cache;

		public SupplierController(RedisCacheService cache)
		{
			_cache = cache;
		}

		private const int PAGE_SIZE = 10;
		private const string MsgSupplierNotFound = "Supplier not found";
		private const string MsgSuccess = "Success";
		private const string MsgError = "An error occurred";
		private const string MsgIdMismatch = "ID does not match";
		private const string MsgCreateFailed = "Failed to create supplier.";
		private const string MsgHasError = "An error occurred";
		private const string MsgSupplierNameIsRequired = "Supplier name is required";

		[HttpGet("supplier")]
		public async Task<IActionResult> GetSuppliers([FromQuery] SupplierFilterDto viewData)
		{
			viewData = new SupplierFilterDto()
			{
				SearchValue = viewData.SearchValue,
				Page = viewData.Page,
				PageSize = PAGE_SIZE,
			};

			string cacheKey = CacheKeyHelper.Supplier(viewData.SearchValue, viewData.Page);

			var cachedData = await _cache.GetCacheAsync<PaginatedSupplierDto>(cacheKey);
			if (cachedData != null)
			{
				return Ok(new
				{
					response_code = ResponseCodes.Success,
					results = cachedData,
					source = "cache"
				});
			}

			var result = new PaginatedSupplierDto
			{
				SearchValue = viewData.SearchValue,
				CurrentPage = viewData.Page,
				CurrentPageSize = viewData.PageSize,
				TotalRow = await _supplierRepo.Count(viewData),
				Data = await _supplierRepo.LoadSuppliers(viewData)
			};

			await _cache.SetCacheAsync(cacheKey, result, TimeSpan.FromMinutes(5));

			return Ok(new
			{
				response_code = ResponseCodes.Success,
				results = result,
				source = "db"
			});
		}

		[HttpGet("supplier/{supplierid}")]
		public async Task<IActionResult> GetSupplier(int supplierid)
		{
			var supplier = await _supplierRepo.GetSupplierById(supplierid);
			if (supplier == null)
				return NotFound(MsgSupplierNotFound);

			return Ok(new
			{
				response_code = ResponseCodes.Success,
				results = supplier
			});
		}

		[HttpPost("supplier")]
		public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierDto viewData)
		{
			if (viewData == null)
				return BadRequest(MsgHasError);

			if (string.IsNullOrEmpty(viewData.SupplierName))
				return BadRequest(MsgSupplierNameIsRequired);

			try
			{
				var isExists = await _supplierRepo.CheckCreateSupplier(viewData.SupplierName);
				if (isExists)
					return BadRequest(MsgSupplierNotFound);

				var newId = await _supplierRepo.CreateSupplier(viewData);
				if (newId <= 0)
				{
					return BadRequest(new
					{
						response_code = ResponseCodes.BadRequest,
						error = MsgCreateFailed
					});
				}

				return Ok(new
				{
					response_code = ResponseCodes.Created,
					supplier_id = newId,
					results = MsgSuccess
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Error: " + ex.Message);
			}
		}

		[HttpPut("supplier/{supplierid}")]
		public async Task<IActionResult> UpdateSupplier(int supplierid, [FromBody] EditSupplierDto viewData)
		{
			if (supplierid != viewData.SupplierId)
				return BadRequest(MsgIdMismatch);

			var existing = await _supplierRepo.GetSupplierById(supplierid);
			if (existing == null)
				return NotFound(MsgSupplierNotFound);

			var success = await _supplierRepo.UpdateSupplier(viewData);
			if (!success)
				return StatusCode(500, MsgError);

			await _cache.InvalidateCacheByAffectedIdAsync(viewData.SupplierId);

			return Ok(new
			{
				response_code = ResponseCodes.NoContent,
				supplier_id = supplierid,
				results = MsgSuccess
			});
		}

		[HttpDelete("supplier/{supplierid}")]
		public async Task<IActionResult> DeleteSupplier(int supplierid)
		{
			if (supplierid <= 0)
				return BadRequest(MsgSupplierNotFound);

			var deleted = await _supplierRepo.DeleteSupplier(supplierid);
			if (!deleted)
				return BadRequest(MsgSupplierNotFound);

			await _cache.InvalidateCacheByAffectedIdAsync(supplierid);

			return Ok(new
			{
				response_code = ResponseCodes.NoContent,
				supplier_id = supplierid,
				results = MsgSuccess
			});
		}
	}
}
