using System.ComponentModel.DataAnnotations;

namespace SHOP.Models
{
	public class Creditors_account
	{
		[Key]
		public int id { get; set; }

		[Display(Name = "Full names:", Prompt = "Full Names:")]
		[DataType(DataType.Currency)]
		[Required]
		public string Customer_name { get; set; }

		[Display(Name = "Phone number:", Prompt = "Phone number:")]
		[DataType(DataType.Currency)]
		[Required]
		public int Phone_number { get; set; }

		[Display(Name = "Item quantity:", Prompt = "Quantity")]
		[Required]
		public float Credit { get; set; }

		[Required]
		[DataType(DataType.Date)]
		public string Date_created { get; set; }
	}
}
