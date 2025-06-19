namespace SPORTLIGHTS_SERVER.Entities
{
	public class OrderDetail
	{
		/// <summary>
		/// 
		/// </summary>
		public int OrderDetailID { get; set; }
		/// <summary>
		/// Mã đơn hàng
		/// </summary>
		public int OrderID { get; set; }
		/// <summary>
		/// Mã mặt hàng
		/// </summary>
		public int ProductID { get; set; }
		/// <summary>
		/// Tên hàng
		/// </summary>
		public string ProductName { get; set; }
		/// <summary>
		/// Ảnh của hàng
		/// </summary>
		public string Photo { get; set; }
		/// <summary>
		/// Đơn vị tính
		/// </summary>
		public string Unit { get; set; }
		/// <summary>
		/// Số lượng bán
		/// </summary>
		public int Quantity { get; set; } = 0;
		/// <summary>
		/// Giá bán
		/// </summary>
		public decimal SalePrice { get; set; } = 0;

		/// <summary>
		/// tỉnh giao hàng
		/// </summary>
		public string? DeliveryProvince { get; set; }
		/// <summary>
		/// địa chỉ giao hàng
		/// </summary>
		public string? DeliveryAddress { get; set; }

		/// <summary>
		/// Thành tiền = Số lượng * Giá bán
		/// </summary>
		public decimal TotalPrice
		{
			get
			{
				return Quantity * SalePrice;
			}
		}
	}
}
