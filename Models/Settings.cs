using System.ComponentModel.DataAnnotations;

namespace SHOP.Models
{
	public class Settings
	{
		[Key]
		public int id { get; set; }
		public bool Action { get; set; }
		public bool Status { get; set; }
		
		
	}
}
