using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Products;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Products;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Products.Abstractions;
using SPORTLIGHTS_SERVER.Authen.Helpers;
using SPORTLIGHTS_SERVER.Constants;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Controllers
{
	//[Authorize(Roles = WebUserRoles.Administrator)]
	[Route("api/v1/admin")]
	[ApiController]
	public class ProductController : ControllerBase
	{
		private readonly IProductRepository _productRepo = new ProductRepository();

		private const int PAGE_SIZE = 10;
		private const string MsgProductNotFound = "Sản phẩm không tồn tại";
		private const string MsgError = "Có lỗi xảy ra";
		private const string MsgSuccess = "Thành công";



		[HttpGet("product")]
		public IActionResult GetProducts([FromQuery] ProductFilterDto filter)
		{
			filter.PageSize = PAGE_SIZE;

			var result = new PaginatedProductDto
			{
				SearchValue = filter.SearchValue,
				CurrentPage = filter.Page,
				CurrentPageSize = filter.PageSize,
				TotalRow = _productRepo.CountProducts(filter),
				Data = _productRepo.GetProducts(filter)
			};

			return Ok(new
			{
				response_code = ResponseCodes.Success,
				results = result
			});
		}

		[HttpGet("product/{productId}")]
		public IActionResult GetProduct(int productId)
		{
			var product = _productRepo.GetProductById(productId);
			if (product == null)
				return NotFound(MsgProductNotFound);

			return Ok(new
			{
				response_code = ResponseCodes.Success,
				results = product
			});
		}

		[HttpPost("product")]
		public IActionResult CreateProduct([FromBody] CreateProductDto dto)
		{
			if (dto.CategoryId <= 0)
			{
				return StatusCode(500, MsgError);
			}

			var newId = _productRepo.CreateProduct(dto);
			if (newId <= 0)
				return StatusCode(500, MsgError);

			return Ok(new
			{
				response_code = ResponseCodes.Created,
				product_id = newId,
				results = MsgSuccess
			});
		}

		[HttpPut("product/{productId}")]
		public IActionResult UpdateProduct(int productId, [FromBody] EditProductDto dto)
		{
			if (productId != dto.ProductId)
				return BadRequest("ID không khớp");

			var existing = _productRepo.GetProductById(productId);
			if (existing == null)
				return NotFound(MsgProductNotFound);

			var success = _productRepo.UpdateProduct(dto);
			if (!success)
				return StatusCode(500, MsgError);

			return Ok(MsgSuccess);
		}

		[HttpDelete("product/{productId}")]
		public IActionResult DeleteProduct(int productId)
		{
			if (productId <= 0)
				return BadRequest(MsgProductNotFound);

			var deleted = _productRepo.DeleteProduct(productId);
			if (!deleted)
				return BadRequest(MsgProductNotFound);

			return Ok(MsgSuccess);
		}
	}
}
