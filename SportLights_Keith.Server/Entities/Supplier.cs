﻿namespace SPORTLIGHTS_SERVER.Entities
{ /// <summary>
  ///  neu internal thì chỉ dùng trong project , dùng public   internal class Customer
  /// </summary>
	public class Supplier

	{
		public int SupplierID { get; set; }
		public string SupplierName { get; set; } = "";
		public string ContactName { get; set; } = "";
		public string Provice { get; set; } = "";
		public string Address { get; set; } = "";
		public string Phone { get; set; } = "";
		public string Email { get; set; } = "";


	}
}
