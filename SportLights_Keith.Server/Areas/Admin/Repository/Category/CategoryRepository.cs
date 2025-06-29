﻿using Dapper;
using Microsoft.Data.SqlClient;
using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Categories;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.CategoryRepository.Abstractions;
using SPORTLIGHTS_SERVER.Entities;
using SPORTLIGHTS_SERVER.Modules;
using System.Data;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Repository.CategoryRepository
{

	public class CategoryRepository : ICategoryRepository
	{
		public async Task<bool> CheckCreateCategory(string category)
		{
			using (var conn = ConnectDB.LiteCommerceDB())
			{
				var sqlCheckCreateCategory = $@"SELECT CategoryName
		                  FROM Categories
		                  WHERE CategoryName = @CategoryName";

				var parameters = new
				{
					CategoryName = category,
				};

				var command = new CommandDefinition(sqlCheckCreateCategory, parameters: parameters, flags: CommandFlags.NoCache);

				var catelogNameInfo = await conn.QueryFirstOrDefaultAsync<Category>(command);

				return catelogNameInfo != null;
			}
		}

		int ICategoryRepository.Count(ViewFitlerCategory viewData)
		{
			using (var conn = ConnectDB.LiteCommerceDB())
			{
				var sqlGetCountCategory = @"SELECT COUNT(*)
		               FROM Categories 
		               WHERE ( (@SearchValue = N'') /* Default search */
		                  OR CategoryName LIKE @SearchValue ) /* Search via CatelogName */";

				var param = new
				{
					SearchValue = $"%{viewData.SearchValue}%",
					Page = viewData.Page,
					PageSize = viewData.PageSize,
				};

				var command = new CommandDefinition(sqlGetCountCategory, parameters: param, flags: CommandFlags.NoCache);

				var totalRow = conn.QuerySingleOrDefault<int>(command);

				return totalRow;
			}
		}

		public async Task<bool> CreateCategory(CreateCategoryDto dataDto)
		{
			using (var conn = ConnectDB.LiteCommerceDB())
			{
				if (conn.State == ConnectionState.Closed)
				{
					await conn.OpenAsync();
				}

				using (var trans = conn.BeginTransaction())
				{
					try
					{
						var sqlInsertCategory = @"INSERT INTO Categories(
					CategoryName, Description
				) VALUES (
					@CategoryName, @Description
				)";

						var parameters = new
						{
							CategoryName = dataDto.CategoryName,
							Description = dataDto.CategoryDescription
						};

						var command = new CommandDefinition(
							sqlInsertCategory,
							parameters: parameters,
							flags: CommandFlags.NoCache,
							transaction: trans,
							cancellationToken: default
						);

						await conn.ExecuteAsync(command);
						trans.Commit();
						return true;
					}
					catch (SqlException)
					{
						trans.Rollback();
						throw;
					}
				}
			}
		}

		public async Task<bool> DeleteCategory(long categoryId)
		{
			int result = 0;

			using (var conn = ConnectDB.LiteCommerceDB())
			{
				var deleteCategorySql = $@"DELETE FROM Categories 
		                  WHERE CategoryId = @CategoryId";

				var parameters = new
				{
					CategoryId = categoryId,
				};

				var command = new CommandDefinition(deleteCategorySql, parameters: parameters, flags: CommandFlags.NoCache);
				result = await conn.ExecuteAsync(command);
			}

			return result == 1;
		}

		public async Task<IReadOnlyList<Category>> GetCategory()
		{
			using (var conn = ConnectDB.LiteCommerceDB())
			{
				var sqlGetCategories = $@"SELECT
		                      CategoryID,
		                      CategoryName,
		                      Description
		                      FROM  Categories";
				var categories = (await conn.QueryAsync<Category>(sqlGetCategories)).ToList();
				return categories;
			}
		}

		public async Task<Category> GetCategorys(long categoryId)
		{
			using (var conn = ConnectDB.LiteCommerceDB())
			{
				var sqlGetCategories = $@"SELECT
		                      CategoryID,
		                      CategoryName,
		                      Description
		                      FROM  Categories
		                      Where CategoryId = @CategoryId";
				var param = new
				{
					CategoryId = categoryId,
				};

				var command = new CommandDefinition(sqlGetCategories, parameters: param, flags: CommandFlags.NoCache);
				var data = await conn.QueryFirstOrDefaultAsync<Category>(command);
				return data;
			}
		}	

		public bool UpdateCategory(EditCategoryDto dataDto)
		{
			int result = 0;

			using (var conn = ConnectDB.LiteCommerceDB())
			{
				if (conn.State == ConnectionState.Closed)
				{
					conn.Open();
				}

				using (var trans = conn.BeginTransaction())
				{
					try
					{
						var updateProductSql = $@"UPDATE Categories
		                      SET CategoryName = @CategoryName , Description = @Description 
		                      WHERE CategoryId = @CategoryId";

						var parameters = new
						{
							CategoryId = dataDto.CategoryId,
							CategoryName = dataDto.CategoryName,
							Description = dataDto.CategoryDescription,
						};

						var command = new CommandDefinition(updateProductSql, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);
						result = conn.Execute(command);

						trans.Commit();
					}
					catch (SqlException)
					{
						trans.Rollback();
						throw;
					}
				}
			}
			return result == 1;
		}

		public async Task<IReadOnlyList<Category>> LoadCategory(ViewFitlerCategory viewData)
		{
			using (var conn = ConnectDB.LiteCommerceDB())
			{
				var sqlloadcategorys = $@"{GenCategorytPaginateCTE()}
		                  /* call cte */
				    select categoryid 
		                  , categoryname
		                  , description    
		                  from categorytpaginatecte 
		                  where (@Page = 1 and @PageSize = 0) or (@Page = 0 and @PageSize = 0) /* no paginate */
		            or rownum between ((@Page - 1) * @PageSize + 1) and (@Page * @PageSize) /* paginate via page and pagesize */";

				var param = new
				{
					SearchValue = viewData.SearchValue,
					Page = viewData.Page,
					PageSize = viewData.PageSize,
				};

				var command = new CommandDefinition(sqlloadcategorys, parameters: param, flags: CommandFlags.NoCache);

				var data = (await conn.QueryAsync<Category>(command)).ToList();
				return data;
			}
		}

		private static string GenCategorytPaginateCTE()
		{
			var sqlCategoryPaginateCTE = @";WITH CategorytPaginateCTE AS (
		               SELECT c.CategoryID 
		                  , c.CategoryName
		                  , c.Description
		                  , ROW_NUMBER() OVER (ORDER BY CategoryID ) AS RowNum	
		               FROM Categories c	                                           
		               WHERE (((@SearchValue = N'') /* Default search */
		                  OR (CategoryName COLLATE Vietnamese_CI_AI LIKE @SearchValue))) /* Search via CatelogName */
		          )";

			return sqlCategoryPaginateCTE;
		}


	}
}
