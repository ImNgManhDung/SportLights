using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Employees;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Employees;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Employees.Abstractions;
using SPORTLIGHTS_SERVER.Authen.Helpers;
using SPORTLIGHTS_SERVER.Constants;
using SPORTLIGHTS_SERVER.Modules;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Controllers
{
	//[Authorize(Roles = WebUserRoles.Administrator)]
	[Route("api/v1/admin")]
	[ApiController]
	public class EmployeeController : ControllerBase
	{
		private readonly IWebHostEnvironment _env;

		public EmployeeController(IWebHostEnvironment env)
		{
			_env = env;
		}

		private readonly IEmployeeRepository _employeeRepo = new EmployeeRepository();

		private const int PAGE_SIZE = 10;
		private const string MsgEmployeeNotFound = "Nhân viên không tồn tại";
		private const string MsgEmployeeNameRequired = "Tên nhân viên không được để trống";
		private const string MsgError = "Có lỗi xảy ra";
		private const string MsgSuccess = "Thành công";

		[HttpGet("employee")]
		public IActionResult GetEmployees([FromQuery] EmployeeFilterDto filter)
		{
			filter.PageSize = PAGE_SIZE;

			var result = new PaginatedEmployeeDto
			{
				SearchValue = filter.SearchValue,
				CurrentPage = filter.Page,
				CurrentPageSize = filter.PageSize,
				TotalRow = _employeeRepo.Count(filter),
				Data = _employeeRepo.GetEmployees(filter)
			};

			return Ok(new
			{
				response_code = ResponseCodes.Success,
				results = result
			});
		}

		[HttpGet("employee/{id}")]
		public IActionResult GetEmployee(int id)
		{
			var employee = _employeeRepo.GetEmployeeById(id);
			if (employee == null)
				return NotFound(MsgEmployeeNotFound);

			return Ok(employee);
		}

		[HttpPost("employee")]
		[Consumes("multipart/form-data")]
		public IActionResult CreateEmployee([FromForm] CreateEmployeeRequestDto form)
		{
			if (string.IsNullOrWhiteSpace(form.FullName))
				return BadRequest(MsgEmployeeNameRequired);

			DateTime? birthDate = Converter.StringToDateTime(form.Birthday);
			if (birthDate == null)
				return BadRequest("Ngày sinh không hợp lệ");

			string? fileName = null;
			if (form.UploadPhoto != null)
			{
				fileName = $"{DateTime.Now.Ticks}_{form.UploadPhoto.FileName}";
				string folder = Path.Combine(_env.WebRootPath, "images", "employees");
				if (!Directory.Exists(folder))
				{
					Directory.CreateDirectory(folder);
				}

				string filePath = Path.Combine(folder, fileName);
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					form.UploadPhoto.CopyTo(stream);
				}
			}

			var dto = new CreateEmployeeDto
			{
				FullName = form.FullName,
				Address = form.Address,
				Email = form.Email,
				BirthDate = birthDate.Value,
				Photo = fileName
			};

			var newId = _employeeRepo.CreateEmployee(dto);
			if (newId <= 0)
				return StatusCode(500, MsgError);

			return Ok(new
			{
				response_code = ResponseCodes.Created,
				employee_id = newId,
				results = MsgSuccess
			});
		}

		[HttpPut("employee/{id}")]
		[Consumes("multipart/form-data")]
		public IActionResult UpdateEmployee(int id, [FromForm] EditEmployeeRequestDto form)
		{
			if (id != form.EmployeeId)
				return BadRequest(MsgError);

			var existing = _employeeRepo.GetEmployeeById(id);
			if (existing == null)
				return NotFound(MsgEmployeeNotFound);

			DateTime? birthDate = Converter.StringToDateTime(form.Birthday);
			if (birthDate == null)
				return BadRequest("Ngày sinh không hợp lệ");

			string? fileName = existing.Photo;
			if (form.UploadPhoto != null)
			{
				fileName = $"{DateTime.Now.Ticks}_{form.UploadPhoto.FileName}";
				string folder = Path.Combine(_env.WebRootPath, "images", "employees");
				if (!Directory.Exists(folder))
				{
					Directory.CreateDirectory(folder);
				}

				string filePath = Path.Combine(folder, fileName);
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					form.UploadPhoto.CopyTo(stream);
				}
			}

			var dto = new EditEmployeeDto
			{
				EmployeeId = form.EmployeeId,
				FullName = form.FullName,
				Address = form.Address,
				Email = form.Email,
				BirthDate = birthDate.Value,
				Photo = fileName
			};

			var success = _employeeRepo.UpdateEmployee(dto);
			if (!success)
				return StatusCode(500, MsgError);

			return Ok(MsgSuccess);
		}

		[HttpDelete("employee/{id}")]
		public IActionResult DeleteEmployee(int id)
		{
			if (id <= 0)
				return BadRequest(MsgEmployeeNotFound);

			var deleted = _employeeRepo.DeleteEmployee(id);
			if (!deleted)
				return BadRequest(MsgEmployeeNotFound);

			return Ok(MsgSuccess);
		}
	}
}
