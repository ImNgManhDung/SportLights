﻿namespace SPORTLIGHTS_SERVER.Entities
{
	public class Product
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
	}

	public class ProductPhoto
	{
		public long PhotoId { get; set; }
		public int ProductId { get; set; }
		public string Photo { get; set; } = "";
		public string Description { get; set; } = "";
		public int DisplayOrder { get; set; }
		public bool IsHidden { get; set; }
	}

	public class ProductAttribute
	{
		public long AttributeId { get; set; }
		public int ProductId { get; set; }
		public string AttributeName { get; set; } = "";
		public string AttributeValue { get; set; } = "";
		public int DisplayOrder { get; set; }
	}


}
