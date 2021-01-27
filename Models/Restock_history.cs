using System.ComponentModel.DataAnnotations;

namespace SHOP.Models
{
	public class Restock_history
	{
		[Required]
		public int Item_id { get; set; }

		[Required]
		public string Date_restock { get; set; }

		[Required]
		public float new_quanity { get; set; }

		[Required]
		public float Prev_quantity { get; set; }

		[Required]
		public float quantity { get; set; }

		[Key]
		public int id { get; set; }
	}
}
