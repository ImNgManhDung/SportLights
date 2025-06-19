using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Suppliers;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Suppliers;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Suppliers.Abstractions;
using SPORTLIGHTS_SERVER.Authen.Helpers;
using SPORTLIGHTS_SERVER.Constants;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Controllers
{
	//[Authorize(Roles = WebUserRoles.Administrator)]
	[Route("api/v1/admin")]
	[ApiController]
	public class SupplierController : ControllerBase
	{
		private readonly ISupplierRepository _supplierRepo = new SupplierRepository();

		private const int PAGE_SIZE = 10;
		private const string MsgSupplierNotFound = "Nhà cung cấp không tồn tại";
		private const string MsgError = "Có lỗi xảy ra";
		private const string MsgSuccess = "Thành công";

		[HttpGet("supplier")]
		public IActionResult GetSuppliers([FromQuery] SupplierFilterDto filter)
		{
			filter.PageSize = PAGE_SIZE;

			var result = new PaginatedSupplierDto
			{
				SearchValue = filter.SearchValue,
				CurrentPage = filter.Page,
				CurrentPageSize = filter.PageSize,
				TotalRow = _supplierRepo.Count(filter),
				Data = _supplierRepo.GetSuppliers(filter)
			};

			return Ok(new
			{
				response_code = ResponseCodes.Success,
				results = result
			});
		}

		[HttpGet("supplier/{supplierid}")]
		public IActionResult GetSupplier(int supplierid)
		{
			var supplier = _supplierRepo.GetSupplierById(supplierid);
			if (supplier == null)
				return NotFound(MsgSupplierNotFound);

			return Ok(supplier);
		}

		[HttpPost("supplier")]
		public IActionResult CreateSupplier([FromBody] CreateSupplierDto dto)
		{
			var newId = _supplierRepo.CreateSupplier(dto);
			if (newId <= 0)
				return BadRequest(new
				{
					response_code = ResponseCodes.BadRequest,
					error = "Failed to create supplier."
				});

			return Ok(new
			{
				response_code = ResponseCodes.Created,
				supplier_id = newId,
				results = MsgSuccess
			});
		}

		[HttpPut("supplier/{supplierid}")]
		public IActionResult UpdateSupplier(int supplierid, [FromBody] EditSupplierDto dto)
		{
			if (supplierid != dto.SupplierId)
				return BadRequest("ID không khớp");

			var existing = _supplierRepo.GetSupplierById(supplierid);
			if (existing == null)
				return NotFound(MsgSupplierNotFound);

			var success = _supplierRepo.UpdateSupplier(dto);
			if (!success)
				return StatusCode(500, MsgError);

			return Ok(MsgSuccess);
		}

		[HttpDelete("supplier/{supplierid}")]
		public IActionResult DeleteSupplier(int supplierid)
		{
			if (supplierid <= 0)
				return BadRequest(MsgSupplierNotFound);

			var deleted = _supplierRepo.DeleteSupplier(supplierid);
			if (!deleted)
				return BadRequest(MsgSupplierNotFound);

			return Ok(MsgSuccess);
		}
	}
}
