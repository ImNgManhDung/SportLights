namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Products
{
	public class EditProductDto
	{
		public int ProductId { get; set; }
		public string ProductName { get; set; } = "";
		public string ProductDescription { get; set; } = "";
		public int SupplierId { get; set; }
		public int CategoryId { get; set; }
		public string Unit { get; set; } = "";
		public decimal Price { get; set; }
		public string Photo { get; set; } = "";
		public bool IsSelling { get; set; }

		//public long PhotoId { get; set; }

		//public int DisplayOrder { get; set; }
		//public bool IsHidden { get; set; }

		//public long AttributeId { get; set; }

		//public string AttributeName { get; set; } = "";
		//public string AttributeValue { get; set; } = "";



	}
}
