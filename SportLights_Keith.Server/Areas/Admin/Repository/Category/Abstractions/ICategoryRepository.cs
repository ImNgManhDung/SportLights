using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Categories;
using SPORTLIGHTS_SERVER.Entities;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Repository.CategoryRepository.Abstractions
{

	public interface ICategoryRepository
	{
		IReadOnlyList<Category> GetCategory();

		IReadOnlyList<Category> LoadCategory(ViewFitlerCategory viewData);

		int Count(ViewFitlerCategory viewData);

		Category GetCategorys(long id);

		Task<bool> CheckCreateCategory(string catelogName);

		Task<bool> CreateCategory(CreateCategoryDto dataDto);

		bool UpdateCategory(CreateCategoryDto dataDto);

		bool DeleteCategory(long catelogID);

	}
}
