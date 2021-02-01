using System.ComponentModel.DataAnnotations;

namespace SHOP.Models
{
	public class Credits
	{
		[Key]
		public int id { get; set; }

		
		public int Client_id { get; set; }

	
		public string Item { get; set; }
		public string Quantity { get; set; }
		public float Total { get; set; }

	
		public string Date_created { get; set; }
	}
}
