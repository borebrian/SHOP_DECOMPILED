using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHOP.Models
{
	public class Users
	{
		[Required]
		[Display(Name = "Full names", Prompt = "Full names")]
		public string Full_name { get; set; }

		[Required]
		[Display(Name = "Phone number", Prompt = "Phone number")]
		[DataType(DataType.PhoneNumber)]
		public string Phone { get; set; }

		[Required]
		[Display(Name = "Password", Prompt = "Password")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Key]
		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[Display(Name = "National id", Prompt = "Natianal id")]
		public int National_id { get; set; }

		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[Display(Name = "Select role", Prompt = "")]
		public int strRole { get; set; }
	}
}
