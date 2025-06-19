
using System.Globalization;

namespace SPORTLIGHTS_SERVER.Modules
{
	public static class Converter
	{
		public static DateTime? StringToDateTime(string s, string formats = "d/M/yyyy;d-M-yyyy;d.M.yyyy")
		{
			try
			{
				return DateTime.ParseExact(s, formats.Split(';'), CultureInfo.InvariantCulture);
			}
			catch
			{
				return null;
			}
		}
	}
}
