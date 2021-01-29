using System.ComponentModel.DataAnnotations;

namespace SHOP.Models
{
	public class shop_items
	{
		[Key]
		public int id { get; set; }

		[Display(Name = "Item name:", Prompt = "Item name")]
		[DataType(DataType.Currency)]
		[Required]
		public string Item_name { get; set; }

		[Display(Name = "Item price:", Prompt = "Price in Ksh.")]
		[DataType(DataType.Currency)]
		[Required]
		public float Item_price { get; set; }

		[Display(Name = "Item quantity:", Prompt = "Quantity")]
		[Required]
		public float Quantity { get; set; }

		[Required]
		[DataType(DataType.Date)]
		public string DateTime { get; set; }
	
		public float Cost_price { get; set; }
	}
}
