using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Shippers;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Shippers;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Shippers.Abstractions;
using SPORTLIGHTS_SERVER.Authen.Helpers;
using SPORTLIGHTS_SERVER.Constants;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Controllers
{
	//[Authorize(Roles = WebUserRoles.Administrator)]
	[Route("api/v1/admin")]
	[ApiController]
	public class ShipperController : ControllerBase
	{
		private readonly IShipperRepository _shipperRepo = new ShipperRepository();

		private const int PAGE_SIZE = 10;
		private const string MsgShipperNotFound = "Shipper không tồn tại";
		private const string MsgError = "Có lỗi xảy ra";
		private const string MsgSuccess = "Thành công";

		[HttpGet("shipper")]
		public IActionResult GetShippers([FromQuery] ShipperFilterDto filter)
		{
			filter.PageSize = PAGE_SIZE;

			var result = new PaginatedShipperDto
			{
				SearchValue = filter.SearchValue,
				CurrentPage = filter.Page,
				CurrentPageSize = filter.PageSize,
				TotalRow = _shipperRepo.Count(filter),
				Data = _shipperRepo.GetShippers(filter)
			};

			return Ok(new
			{
				response_code = ResponseCodes.Success,
				results = result
			});
		}

		[HttpGet("shipper/{shipperid}")]
		public IActionResult GetShipper(int shipperid)
		{
			var shipper = _shipperRepo.GetShipperById(shipperid);
			if (shipper == null)
				return NotFound(MsgShipperNotFound);

			return Ok(shipper);
		}

		[HttpPost("shipper")]
		public IActionResult CreateShipper([FromBody] CreateShipperDto dto)
		{
			var newId = _shipperRepo.CreateShipper(dto);
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
		public IActionResult UpdateShipper(int shipperid, [FromBody] EditShipperDto dto)
		{
			if (shipperid != dto.ShipperId)
				return BadRequest("ID không khớp");

			var existing = _shipperRepo.GetShipperById(shipperid);
			if (existing == null)
				return NotFound(MsgShipperNotFound);

			var success = _shipperRepo.UpdateShipper(dto);
			if (!success)
				return StatusCode(500, MsgError);

			return Ok(MsgSuccess);
		}

		[HttpDelete("shipper/{shipperid}")]
		public IActionResult DeleteShipper(int shipperid)
		{
			if (shipperid <= 0)
				return BadRequest(MsgShipperNotFound);

			var deleted = _shipperRepo.DeleteShipper(shipperid);
			if (!deleted)
				return BadRequest(MsgShipperNotFound);

			return Ok(MsgSuccess);
		}
	}
}
