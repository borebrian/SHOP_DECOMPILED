using System.ComponentModel.DataAnnotations;

namespace SHOP.Models
{
	public class join_sold_item
	{
		public string Item_id { get; set; }

		public string Item_name { get; set; }

		public float Item_price { get; set; }
		public float Total_Cost_cash { get; set; }


		public float quantity_sold { get; set; }

		public float Total_cash_made { get; set; }
		public float id { get; set; }

		[Required]
		[DataType(DataType.Date)]
		public string DateTime { get; set; }
	}
}
