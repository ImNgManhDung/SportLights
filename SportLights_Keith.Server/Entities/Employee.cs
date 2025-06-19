namespace SPORTLIGHTS_SERVER.Entities
{ /// <summary>
  ///  neu internal thì chỉ dùng trong project , dùng public   internal class Customer
  /// </summary>
	public class Employee

	{
		public int EmployeeId { get; set; }
		public string FullName { get; set; } = "";
		public DateTime BirthDate { get; set; }
		public string Address { get; set; } = "";
		public string Phone { get; set; } = "";
		public string Email { get; set; } = "";
		public string Photo { get; set; } = "";
		public bool IsWorking { get; set; }

	}
}
