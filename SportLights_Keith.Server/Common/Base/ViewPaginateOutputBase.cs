namespace SPORTLIGHTS_SERVER.Common.Base
{
	public class ViewPaginateOutputBase<T>
		where T : class
	{
		public int CurrentPage { get; set; }

		public int CurrentPageSize { get; set; }

		public int TotalRow { get; set; }

		public int TotalPage
		{
			get
			{
				return (int)Math.Ceiling((double)TotalRow / CurrentPageSize);
			}
		}

		public IReadOnlyList<T> Data { get; set; }
	}
}

