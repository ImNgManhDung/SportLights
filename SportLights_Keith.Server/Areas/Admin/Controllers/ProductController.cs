using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Products;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Products;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Products.Abstractions;
using SPORTLIGHTS_SERVER.Authen.Helpers;
using SPORTLIGHTS_SERVER.Constants;
using SPORTLIGHTS_SERVER.Modules;
using System.Data;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Controllers
{
	// [Authorize(Roles = WebUserRoles.Administrator)]
	[Route("api/v1/admin")]
	[ApiController]
	public class ProductController : ControllerBase
	{
		private readonly IProductRepository _productRepo = new ProductRepository();
		private readonly RedisCacheService _cache;

		public ProductController(RedisCacheService cache)
		{
			_cache = cache;
		}

		private const int PAGE_SIZE = 10;
		private const string MsgProductNotFound = "Product Not Found";
		private const string MsgError = "An error occurred";
		private const string MsgSuccess = "Success";
		private const string IdMisMatch = "Id mismatch";


		[HttpGet("product")]
		public async Task<IActionResult> GetProducts([FromQuery] ProductFilterDto viewData)
		{
			viewData = new ProductFilterDto
			{
				SearchValue = viewData.SearchValue,
				Page = viewData.Page, 
				PageSize = PAGE_SIZE
			};		

			string cacheKey = CacheKeyHelper.Product(viewData.SearchValue, viewData.Page);

			var cachedData = await _cache.GetCacheAsync<PaginatedProductDto>(cacheKey);
			if (cachedData != null)
			{
				return Ok(new
				{
					response_code = ResponseCodes.Success,
					results = cachedData,
					source = "cache"
				});
			}
			var data = await _productRepo.GetProducts(viewData);

			var result = new PaginatedProductDto
			{
				SearchValue = viewData.SearchValue,
				CurrentPage = viewData.Page,
				CurrentPageSize = viewData.PageSize,
				TotalRow = await _productRepo.CountProducts(viewData),
				Data = data
			};

			var relateId = data.Select(p => p.ProductId).ToList();

			await _cache.SetCacheWithIdsAsync(cacheKey, result ,relateId, TimeSpan.FromMinutes(5));

			//await _cache.SetCacheAsync(cacheKey, result, TimeSpan.FromMinutes(5));

			return Ok(new
			{
				response_code = ResponseCodes.Success,
				results = result,
				source = "db"
			});
		}

		[HttpGet("product/{productId}")]
		public async Task<IActionResult> GetProduct(int productId)
		{
			var product = await _productRepo.GetProductById(productId);
			if (product == null)
				return NotFound(MsgProductNotFound);

			return Ok(new
			{
				response_code = ResponseCodes.Success,
				results = product
			});
		}

		[HttpPost("product")]
		public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto viewData)
		{
			if (viewData.CategoryId <= 0)
				return StatusCode(500, MsgError);

			var newId = await _productRepo.CreateProduct(viewData);
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
		public async Task<IActionResult> UpdateProduct(int productId, [FromBody] EditProductDto viewData)
		{
			if (productId != viewData.ProductId)
				return BadRequest(IdMisMatch);

			var existing = _productRepo.GetProductById(productId);
			if (existing == null)
				return NotFound(MsgProductNotFound);

			var success = await _productRepo.UpdateProduct(viewData);
			if (!success)
				return StatusCode(500, MsgError);

			await _cache.InvalidateCacheByAffectedIdAsync(viewData.ProductId);

			return Ok(new
			{
				response_code = ResponseCodes.NoContent,
				product_id = productId,
				results = MsgSuccess
			});
		}

		[HttpDelete("product/{productId}")]
		public async Task<IActionResult> DeleteProduct(int productId)
		{
			if (productId <= 0)
				return BadRequest(MsgProductNotFound);

			var deleted = await _productRepo.DeleteProduct(productId);
			if (!deleted)
				return BadRequest(MsgProductNotFound);

			await _cache.InvalidateCacheByAffectedIdAsync(productId);

			return Ok(new
			{
				response_code = ResponseCodes.NoContent,
				product_id = productId,
				results = MsgSuccess
			});
		}
	}
}
