using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Categories;
using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Employees;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Employees;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Employees.Abstractions;
using SPORTLIGHTS_SERVER.Authen.Helpers;
using SPORTLIGHTS_SERVER.Constants;
using SPORTLIGHTS_SERVER.Entities;
using SPORTLIGHTS_SERVER.Modules;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Controllers
{
	[Authorize(Roles = WebUserRoles.Administrator)]
	[Route("api/v1/admin")]
	[ApiController]
	public class EmployeeController : ControllerBase
	{
		private readonly IWebHostEnvironment _env;
		private readonly RedisCacheService _cache;	

		public EmployeeController(IWebHostEnvironment env, RedisCacheService cache)
		{
			_env = env;
			_cache = cache;
		}

		private readonly IEmployeeRepository _employeeRepo = new EmployeeRepository();

		private const int PAGE_SIZE = 10;
		private const string MsgEmployeeNotFound = "Employee not found";
		private const string MsgEmployeeNameRequired = "Employee name is required";
		private const string MsgError = "An error has occurred";
		private const string MsgSuccess = "Success";
		private const string MsgInvalidBirthDate = "Invalid date of birth";


		[HttpGet("employee")]
		public async Task<IActionResult> GetEmployees([FromQuery] EmployeeFilterDto viewData)
		{
			viewData = new EmployeeFilterDto()
			{
				SearchValue = viewData.SearchValue,
				Page = viewData.Page,
				PageSize = PAGE_SIZE,
			};

			string cacheKey = CacheKeyHelper.Employee(viewData.SearchValue, viewData.Page);

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

			var result = new PaginatedEmployeeDto
			{
				SearchValue = viewData.SearchValue,
				CurrentPage = viewData.Page,
				CurrentPageSize = viewData.PageSize,
				TotalRow = _employeeRepo.Count(viewData),
				Data = await _employeeRepo.LoadEmployees(viewData)
			};

			var relatedIds = result.Data.Select(e => e.EmployeeId).ToList();

			await _cache.SetCacheWithIdsAsync(cacheKey, result, relatedIds, TimeSpan.FromMinutes(5));	

			//await _cache.SetCacheAsync(cacheKey, result, TimeSpan.FromMinutes(5));
			
			return Ok(new
			{
				response_code = ResponseCodes.Success,
				results = result,
				source = "db"
			});
		}

		[HttpGet("employee/{employeeId}")]
		public IActionResult GetEmployee(int employeeId)
		{
			var employee = _employeeRepo.GetEmployeeById(employeeId);
			if (employee == null)
				return NotFound(MsgEmployeeNotFound);

			return Ok(new
			{
				response_code = ResponseCodes.Success,
				results = employee,
			});
		}

		[HttpPost("employee")]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> CreateEmployee([FromForm] CreateEmployeeRequestDto viewData)
		{
			if (string.IsNullOrWhiteSpace(viewData.FullName))
				return BadRequest(MsgEmployeeNameRequired);

			DateTime? birthDate = Converter.StringToDateTime(viewData.Birthday);
			if (birthDate == null)
				return BadRequest(MsgInvalidBirthDate);

			string? fileName = null;
			if (viewData.UploadPhoto != null)
			{
				fileName = $"{DateTime.Now.Ticks}_{viewData.UploadPhoto.FileName}";
				string folder = Path.Combine(_env.WebRootPath, "images", "employees");
				if (!Directory.Exists(folder))
				{
					Directory.CreateDirectory(folder);
				}

				string filePath = Path.Combine(folder, fileName);
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					viewData.UploadPhoto.CopyTo(stream);
				}
			}

			var createEmployeeDto = new CreateEmployeeDto
			{
				FullName = viewData.FullName,
				BirthDate = birthDate.Value,
				Address = viewData.Address,
				Phone = viewData.Phone,
				Email = viewData.Email,
				Photo = fileName
			};

			var newCreateId = await _employeeRepo.CreateEmployee(createEmployeeDto);
			if (newCreateId <= 0)
				return StatusCode(500, MsgError);

			return Ok(new
			{
				response_code = ResponseCodes.Created,
				employee_id = newCreateId,
				results = MsgSuccess
			});
		}

		[HttpPut("employee/{employeeId}")]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> UpdateEmployee(int employeeId, [FromForm] EditEmployeeRequestDto viewData)
		{
			if (employeeId != viewData.EmployeeId)
				return BadRequest(MsgError);

			var existing = await _employeeRepo.GetEmployeeById(employeeId);
			if (existing == null)
				return NotFound(MsgEmployeeNotFound);

			DateTime? birthDate = Converter.StringToDateTime(viewData.Birthday);
			if (birthDate == null)
				return BadRequest(MsgInvalidBirthDate);

			string? fileName = existing.Photo;
			if (viewData.UploadPhoto != null)
			{
				fileName = $"{DateTime.Now.Ticks}_{viewData.UploadPhoto.FileName}";
				string folder = Path.Combine(_env.WebRootPath, "images", "employees");
				if (!Directory.Exists(folder))
				{
					Directory.CreateDirectory(folder);
				}

				string filePath = Path.Combine(folder, fileName);
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					viewData.UploadPhoto.CopyTo(stream);
				}
			}

			var editEmployeeDto = new EditEmployeeDto
			{
				EmployeeId = viewData.EmployeeId,
				FullName = viewData.FullName,
				Address = viewData.Address,
				Email = viewData.Email,
				BirthDate = birthDate.Value,
				Photo = fileName
			};

			var IsUpdated = await _employeeRepo.UpdateEmployee(editEmployeeDto);
			if (!IsUpdated)
				return StatusCode(500, MsgError);

			await _cache.InvalidateCacheByAffectedIdAsync(editEmployeeDto.EmployeeId);

			return Ok(new
			{
				response_code = ResponseCodes.NoContent,
				employee_id = employeeId,
				results = MsgSuccess
			});
		}

		[HttpDelete("employee/{employeeId}")]
		public async Task<IActionResult> DeleteEmployee(int employeeId)
		{
			if (employeeId <= 0)
				return BadRequest(MsgEmployeeNotFound);

			var deleted = await _employeeRepo.DeleteEmployee(employeeId);
			if (!deleted)
				return BadRequest(MsgEmployeeNotFound);
				
			await _cache.InvalidateCacheByAffectedIdAsync(employeeId);

			return Ok(new
			{
				response_code = ResponseCodes.NoContent,
				employee_id = employeeId,
				results = MsgSuccess
			});
		}
	}
}
