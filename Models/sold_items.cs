using System.ComponentModel.DataAnnotations;

namespace SHOP.Models
{
	public class sold_items
	{
		[Key]
		public int id { get; set; }

		public int Item_id { get; set; }

		public float quantity_sold { get; set; }

		public float Total_cash_made { get; set; }

		[Required]
		[DataType(DataType.Date)]
		public string DateTime { get; set; }
	}
}
