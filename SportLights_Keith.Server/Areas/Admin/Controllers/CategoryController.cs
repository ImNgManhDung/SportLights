using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Categories;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.CategoryRepository;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.CategoryRepository.Abstractions;
using SPORTLIGHTS_SERVER.Authen.Helpers;
using SPORTLIGHTS_SERVER.Constants;
using SPORTLIGHTS_SERVER.Entities;
using SPORTLIGHTS_SERVER.Modules;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Controllers
{
	[Authorize(Roles = WebUserRoles.Administrator)]
	[Route("api/v1/admin")]
	[ApiController]
	public class CategoryController : ControllerBase
	{
		#region Repository
		private readonly ICategoryRepository _categoryRepo = new CategoryRepository();

		private readonly RedisCacheService _cache;

		public CategoryController(RedisCacheService cache)
		{
			_cache = cache;
		}

		#endregion
		#region Constant
		private const int PAGE_SIZE = 5;
		private const string MsgCategoryIsExists = "Category already exists";
		private const string MsgCategoryNameIsExists = "Category name already exists";
		private const string MsgCategoryNameIsRequired = "Category name is required";
		private const string MsgSuccess = "Created successfully";
		private const string MsgCategoryIsNotExists = "Category does not exist";
		private const string MsgHasError = "An error occurred";
		#endregion

		[HttpGet("category")]
		public async Task<IActionResult> GetCategory([FromQuery] ViewFitlerCategory viewData)
		{ 
			viewData = new ViewFitlerCategory()
			{
				SearchValue = viewData.SearchValue,
				Page = viewData.Page,
				PageSize = PAGE_SIZE,
			};
			string cacheKey = CacheKeyHelper.Category(viewData.SearchValue, viewData.Page);

			var cachedData = await _cache.GetCacheAsync<ViewCategory>(cacheKey);
			if (cachedData != null)
			{
				return Ok(new
				{
					response_code = ResponseCodes.Success,
					results = cachedData,
					source = "cache"
				});
			}

			var model = new ViewCategory()
			{
				SearchValue = viewData.SearchValue,
				CurrentPage = viewData.Page,
				CurrentPageSize = viewData.PageSize,
				TotalRow = _categoryRepo.Count(viewData),
				Data = _categoryRepo.LoadCategory(viewData),
			};

			await _cache.SetCacheAsync(cacheKey, model, TimeSpan.FromMinutes(5));

			return Ok(new
			{
				response_code = ResponseCodes.Success,
				results = model,
				source = "db"
			});
		}

		[HttpPost("category")]
		public async Task<ActionResult> Add(CreateCategoryDto model)
		{
			if (model == null)
			{
				return BadRequest(MsgHasError);
			}

			if (string.IsNullOrEmpty(model.CategoryName))
			{
				return BadRequest(MsgCategoryNameIsRequired);
			}

			try
			{
				var isCheckCategoryIsExists = await _categoryRepo.CheckCreateCategory(model.CategoryName);
				if (isCheckCategoryIsExists)
				{
					return BadRequest(MsgCategoryIsExists);
				}

				var isCreated = await _categoryRepo.CreateCategory(model);

				if (!isCreated)
				{
					return BadRequest(MsgHasError);
				}

				return Ok(new
				{
					response_code = ResponseCodes.Created,
					customer_id = isCreated,
					results = MsgSuccess
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Eror: " + ex.Message);
			}
		}

		[HttpPut("{categoryid}")]
		public async Task<ActionResult> Edit(CreateCategoryDto dataView)
		{
			if (dataView == null)
			{
				return BadRequest(MsgHasError);
			}

			var catelogProduct = _categoryRepo.GetCategorys(dataView.CategoryId);

			if (catelogProduct == null)
			{
				return BadRequest(MsgCategoryIsNotExists);

			}

			if (string.IsNullOrEmpty(dataView.CategoryName))
			{
				return BadRequest(MsgCategoryNameIsRequired);
			}

			var isCheckCatelogProductIsExists = await _categoryRepo.CheckCreateCategory(dataView.CategoryName);
			if (isCheckCatelogProductIsExists)
			{
				return BadRequest(MsgCategoryNameIsExists);
			}

			var isUpdated = _categoryRepo.UpdateCategory(dataView);
			if (!isUpdated)
			{
				return BadRequest(MsgHasError);
			}

			return Ok(new
			{
				response_code = ResponseCodes.NoContent,
				category_id = dataView.CategoryId,
			});
		}

		[HttpDelete("{categoryid}")]
		public async Task<ActionResult> Delete(long categoryid = 0)
		{
			if (categoryid == 0)
			{				
				return BadRequest(MsgCategoryIsNotExists);
			}

			var catelogProduct = await _categoryRepo.DeleteCategory(categoryid);

			if (!catelogProduct)
			{
				return BadRequest(MsgCategoryIsNotExists);
			}

			return Ok(new
			{
				response_code = ResponseCodes.NoContent,
				category_id = categoryid,					
			});
		}
	}
}
