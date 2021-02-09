using System.ComponentModel.DataAnnotations;

namespace SHOP.Models
{
	public class Expiries
	{
		[Key]
		public int id { get; set; }
		public string Item_name { get; set; }
		public string Date_created
		
		{ get; set; }
		public string Expiry_date { get; set; }

	}
}
