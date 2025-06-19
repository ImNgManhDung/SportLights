namespace SPORTLIGHTS_SERVER.Entities
{ /// <summary>
  ///  neu internal thì chỉ dùng trong project , dùng public   internal class Customer
  /// </summary>
    public class Customer
    {
		public int CustomerID { get; set; }
        public string CustomerName { get; set; } = "";
        public string ContactName { get; set; } = "";
		public string Province { get; set; } = "";
		public string Address { get; set; } = "";
		public string Phone { get; set; } = "";
		public string Email { get; set; } = "";
		public bool IsLocked { get; set; }

	}
}
