using SPORTLIGHTS_SERVER.Entities;

namespace SPORTLIGHTS_SERVER.Authen.Repository.Auth.Abstractions
{
	public interface IAuthRepository
	{
		Task<UserAccount?> AuthorizeAsync(string userName, string password, TypeOfAccount typeOfAccount);
		Task<UserAccount> GetUserByIdAsync(object userId);

	}
	public enum TypeOfAccount
	{
		Employee,
		Customer
	}
}
