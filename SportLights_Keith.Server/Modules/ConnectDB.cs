using Microsoft.Data.SqlClient;

namespace SPORTLIGHTS_SERVER.Modules
{
	public static class ConnectDB
	{
		private static readonly string _connectionString;

		static ConnectDB()
		{
			var config = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();

			_connectionString = config.GetConnectionString("LiteCommerceDB");
		}

		public static SqlConnection LiteCommerceDB()
		{
			return new SqlConnection(_connectionString);
		}
	}
}
