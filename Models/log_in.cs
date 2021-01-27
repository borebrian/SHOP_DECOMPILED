using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHOP.Models
{
	public class log_in
	{
		[Required]
		[Display(Name = "Full names", Prompt = "Full names")]
		public string Full_name { get; set; }

		[Required]
		[Display(Name = "Phone number", Prompt = "Phone number")]
		[DataType(DataType.PhoneNumber)]
		public string Phone { get; set; }

		[Required]
		[Display(Name = "Shop name", Prompt = "Shop name")]
		public string Shop_name { get; set; }

		[Required]
		[Display(Name = "Password")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[Display(Name = "Select role", Prompt = "")]
		public int strRole { get; set; }

		[Key]
		public int id { get; set; }
	}
}
