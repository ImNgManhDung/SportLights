using SPORTLIGHTS_SERVER.Entities;

namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Orders
{
	public class CreateOrderDto
	{
		public int OrderID { get; set; }
		/// <summary>
		/// Thời điểm đặt hàng
		/// </summary>
		public DateTime OrderTime { get; set; }
		/// <summary>
		/// Thời điểm chấp nhận đơn hàng
		/// </summary>
		public DateTime? AcceptTime { get; set; } = null;
		/// <summary>
		/// Thời điểm bắt đầu giao hàng
		/// </summary>
		public DateTime? ShippedTime { get; set; }
		/// <summary>
		/// Thời điểm kết thúc quá trình xử lý đối với đơn hàng (là thời điểm khách nhận hàng nếu đơn hàng
		/// được giao, hoặc thời điểm thực hiện việc hủy bỏ/từ chối đơn hàng nếu đơn hàng bị hủy bỏ hoặc từ chối)
		/// </summary>
		public DateTime? FinishedTime { get; set; }
		/// <summary>
		/// Mã trạng thái đơn hàng
		/// </summary>
		public int? Status { get; set; }

		/// <summary>
		/// Mã khách hàng đặt mua hàng
		/// </summary>
		public int? CustomerID { get; set; }
		/// <summary>
		/// Tên khách hàng
		/// </summary>
		public string? CustomerName { get; set; }
		/// <summary>
		/// Tên giao dịch của khách hàng
		/// </summary>
		public string? CustomerContactName { get; set; }
		/// <summary>
		/// Địa chỉ của khách hàng
		/// </summary>
		public string? CustomerAddress { get; set; }
		/// <summary>
		/// Email của khách hàng
		/// </summary>
		public string? CustomerEmail { get; set; }

		public List<OrderDetail> Details { get; set; } = new();

		/// <summary>
		/// Mã của nhân viên phụ trách đơn hàng
		/// </summary>
		public int? EmployeeID { get; set; }
		/// <summary>
		/// Họ tên của nhân viên phụ trách đơn hàng
		/// </summary>
		public string? EmployeeFullName { get; set; }

		/// <summary>
		/// Mã người giao hàng
		/// </summary>
		public int? ShipperID { get; set; } = null;
		/// <summary>
		/// Tên người giao hàng
		/// </summary>
		public string? ShipperName { get; set; }
		/// <summary>
		/// Điện thoại của người giao hàng
		/// </summary>
		public string? ShipperPhone { get; set; }
		/// <summary>
		/// tỉnh giao hàng
		/// </summary>
		public string DeliveryProvince { get; set; }
		/// <summary>
		/// địa chỉ giao hàng
		/// </summary>
		public string DeliveryAddress { get; set; }
		/// <summary>
		/// Mô tả trạng thái đơn hàng dựa trên mã trạng thái
		/// </summary>
	}
}
