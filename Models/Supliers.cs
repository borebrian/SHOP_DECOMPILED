using System.ComponentModel.DataAnnotations;

namespace SHOP.Models
{
	public class Suppliers
	{
		[Key]
		public int id { get; set; }
		public string Company_name { get; set; }	
		public string Phone { get; set; }
		public string Location { get; set; }
		
	}
}
