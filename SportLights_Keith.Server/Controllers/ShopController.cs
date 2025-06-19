using Microsoft.AspNetCore.Mvc;
using SPORTLIGHTS_SERVER.Constants;
using SPORTLIGHTS_SERVER.Models.Shop;
using SPORTLIGHTS_SERVER.Repository.Shop;
using SPORTLIGHTS_SERVER.Repository.Shop.Abstractions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SPORTLIGHTS_SERVER.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ShopController : ControllerBase
	{
		#region Repository
		private readonly IShopRepository _shopRepo = new ShopRepository();
		//private readonly IProductRepository _productRepo = new ProductRepository();
		#endregion
		#region Constant
		private const string STORAGE_ROOT_IMAGES_FOOD = "~/Images/Products";
		private const int PAGE_SIZE = 8;
		private const string KeyErrorMessage = "error_message";
		private const string ControllerName = "ManageProduct";
		private const string ActionIndex = "Index";
		private const string ActionFilter = "Filter";
		private const string ActionAdd = "Add";
		private const string ActionEdit = "Edit";
		private const string ActionSave = "Save";
		private const string ActionSoftDelete = "SoftDelete";
		private const string ActionRestore = "Restore";
		private const string TitleAddProduct = "Thêm Quảng Cáo";
		private const string TitleEditProduct = "Sửa Quảng Cáo";
		private const string ValueErrorUploadFile = "Tệp tải lên phải thuộc các loại sau: .png, .jpg, .gif, .jpeg, .tiff";
		private const string ValueErrorAddProduct = "Có lỗi xảy ra khi thêm mới quảng cáo";
		private const string ValueErrorEditProduct = "Có lỗi xảy ra khi sửa quảng cấo";
		private const string ValueNotUploadFileWhenAdd = "Hình ảnh bắt buộc phải có";
		private const string MsgProductIsNotExists = "Quảng cáo không tồn tại";
		private const string MsgHasError = "Có lỗi";
		private const string MsgFail = "Thất bại";
		private const string MsgSuccess = "Thành công";
		#endregion
		// GET: api/<ShopController>
		[HttpGet]
		public IActionResult GetProducts()
		{
			//int page = 1,
			//int pageSize = PAGE_SIZE,
			//int catelogProductsID = 0,
			//string searchValue = "",
			//string sortField = "Default",
			//string sortType = "1"

			var viewData = new ViewFilterShop()
			{
				SortField = SortField.Default,
				SortType = SortTypeConstant.DEFAULT,
				CatelogProductsID = 0,
				SearchValue = string.Empty,
				Page = 1,
				PageSize = 1,
			};

			var data = _shopRepo.GetProducts(viewData);


			//var result = new ViewFilterShop
			//{
			//    Data = data,

			//};

			return Ok(data);
		}

		// GET api/<ShopController>/5
		[HttpGet("{id}")]
		public string Get(int id)
		{
			return "value";
		}

		// POST api/<ShopController>
		[HttpPost]
		public void Post([FromBody] string value)
		{
		}

		// PUT api/<ShopController>/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/<ShopController>/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}
}
