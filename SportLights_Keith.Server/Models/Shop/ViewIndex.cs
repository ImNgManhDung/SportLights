using SPORTLIGHTS_SERVER.Common.Base;
using SPORTLIGHTS_SERVER.Entities;

namespace SPORTLIGHTS_SERVER.Models.Shop
{
	public class ViewIndex : ViewPaginateOutputBase<Order>
	{
		public string SearchValue { get; set; }

		public int OrderStatus { get; set; }

		public string DateStart { get; set; }

		public string DateEnd { get; set; }
	}
}
