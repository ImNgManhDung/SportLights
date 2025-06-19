using Dapper;
using SPORTLIGHTS_SERVER.Authen.Repository.Auth.Abstractions;
using SPORTLIGHTS_SERVER.Entities;
using SPORTLIGHTS_SERVER.Modules;
using System.Data;

namespace SPORTLIGHTS_SERVER.Authen.Repository.Auth
{
	public class AuthRepository : IAuthRepository
	{
		public async Task<UserAccount?> AuthorizeAsync(string username, string password, TypeOfAccount accountType)
		{
			using var connection = ConnectDB.LiteCommerceDB();

			string sql = accountType switch
			{
				TypeOfAccount.Employee => @"SELECT EmployeeID AS UserId, Email AS UserName, FullName, Email, Photo  
                                             FROM Employees 
                                             WHERE Email = @Username AND Password = @Password",

				TypeOfAccount.Customer => @"SELECT CustomerID AS UserId, Email AS UserName, FullName, Email, Photo  
                                             FROM Customers 
                                             WHERE Email = @Username AND Password = @Password",

				_ => throw new ArgumentException("Loại tài khoản không hợp lệ")
			};

			var parameters = new { Username = username, Password = password };

			return await connection.QueryFirstOrDefaultAsync<UserAccount>(sql, parameters, commandType: CommandType.Text);
		}

		public UserAccount GetUserById(object userId)
		{
			throw new NotImplementedException();
		}

		public Task GetUserByIdAsync(string userId)
		{
			throw new NotImplementedException();
		}

		public Task<UserAccount> GetUserByIdAsync(object userId)
		{
			throw new NotImplementedException();
		}


	}
}
