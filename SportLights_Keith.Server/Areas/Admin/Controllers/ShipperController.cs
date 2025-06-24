using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Shippers;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Shippers;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Shippers.Abstractions;
using SPORTLIGHTS_SERVER.Authen.Helpers;
using SPORTLIGHTS_SERVER.Constants;
using SPORTLIGHTS_SERVER.Modules;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Controllers
{
	// [Authorize(Roles = WebUserRoles.Administrator)]
	[Route("api/v1/admin")]
	[ApiController]
	public class ShipperController : ControllerBase
	{
		private readonly IShipperRepository _shipperRepo = new ShipperRepository();
		private readonly RedisCacheService _cache;

		public ShipperController(RedisCacheService cache)
		{
			_cache = cache;
		}

		private const int PAGE_SIZE = 10;
		private const string MsgShipperNotFound = "Shipper Not Found";
		private const string MsgError = "Error";
		private const string MsgSuccess = "Success";
		private const string MsgIDMisMatch = "ID MisMatch";

		[HttpGet("shipper")]
		public async Task<IActionResult> GetShippers([FromQuery] ShipperFilterDto viewData)
		{
			viewData = new ShipperFilterDto
			{
				SearchValue = viewData.SearchValue,
				Page = viewData.Page,
				PageSize = PAGE_SIZE
			};
			
			string cacheKey = CacheKeyHelper.Shipper(viewData.SearchValue, viewData.Page);

			var cachedData = await _cache.GetCacheAsync<PaginatedShipperDto>(cacheKey);
			if (cachedData != null)
			{
				return Ok(new
				{
					response_code = ResponseCodes.Success,
					results = cachedData,
					source = "cache"
				});
			}

			var data = await _shipperRepo.GetShippers(viewData);

			var result = new PaginatedShipperDto
			{
				SearchValue = viewData.SearchValue,
				CurrentPage = viewData.Page,
				CurrentPageSize = viewData.PageSize,
				TotalRow = await _shipperRepo.Count(viewData),
				Data = data
			};

			var relatedId = data.Select(s => s.ShipperID).ToList();

			await _cache.SetCacheWithIdsAsync(cacheKey,result,relatedId,TimeSpan.FromMinutes(5));

			//await _cache.SetCacheAsync(cacheKey, result, TimeSpan.FromMinutes(5));

			return Ok(new
			{
				response_code = ResponseCodes.Success,
				results = result,
				source = "db"
			});
		}

		[HttpGet("shipper/{shipperid}")]
		public async Task<IActionResult> GetShipper(int shipperid)
		{
			var shipper = await _shipperRepo.GetShipperById(shipperid);
			if (shipper == null)
				return NotFound(MsgShipperNotFound);

			return Ok(new
			{
				response_code = ResponseCodes.Success,
				results = shipper
			});
		}

		[HttpPost("shipper")]
		public async Task<IActionResult> CreateShipper([FromBody] CreateShipperDto viewData)
		{
			var newId = await _shipperRepo.CreateShipper(viewData);
			if (newId <= 0)
				return StatusCode(500, MsgError);

			return Ok(new
			{
				response_code = ResponseCodes.Created,
				shipper_id = newId,
				results = MsgSuccess
			});
		}

		[HttpPut("shipper/{shipperid}")]
		public async Task<IActionResult> UpdateShipper(int shipperid, [FromBody] EditShipperDto viewData)
		{
			if (shipperid != viewData.ShipperId)
				return BadRequest(MsgIDMisMatch);

			var existing = await _shipperRepo.GetShipperById(shipperid);
			if (existing == null)
				return NotFound(MsgShipperNotFound);

			var success = await _shipperRepo.UpdateShipper(viewData);
			if (!success)
				return StatusCode(500, MsgError);

			await _cache.InvalidateCacheByAffectedIdAsync(viewData.ShipperId);

			return Ok(new
			{
				response_code = ResponseCodes.NoContent,
				shipper_id = shipperid,
				results = MsgSuccess
			});
		}

		[HttpDelete("shipper/{shipperid}")]
		public async Task<IActionResult> DeleteShipper(int shipperid)
		{
			if (shipperid <= 0)
				return BadRequest(MsgShipperNotFound);

			var deleted = await _shipperRepo.DeleteShipper(shipperid);
			if (!deleted)
				return BadRequest(MsgShipperNotFound);

			await _cache.InvalidateCacheByAffectedIdAsync(shipperid);

			return Ok(new
			{
				response_code = ResponseCodes.NoContent,
				shipper_id = shipperid,
				results = MsgSuccess
			});
		}
	}
}
