using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace SHOP.Models
{
	public class Dynami_images
	{
		[Key]
		[Required]
		public int id { get; set; }

		[Display(Name = "Shop category:", Prompt = "Please choose shop category")]
		[Required]
		[NotMapped]
		public string Category { get; set; }

		[Display(Name = "Company name:", Prompt = "Enter company name")]
		[Required]
		[NotMapped]
		public string Company_name { get; set; }

		[Display(Name = "Company logo:", Prompt = "Choose logo")]
		[Required]
		[NotMapped]
		public IFormFile Logo { get; set; }

		public string log_url { get; set; }

		[Display(Name = "Choose item icon:", Prompt = "Item icon")]
		[Required]
		[NotMapped]
		public IFormFile item_icon { get; set; }

		public string item_icon_url { get; set; }

		[Display(Name = "Popup icon:", Prompt = "Choose")]
		[Required]
		[NotMapped]
		public IFormFile popup_icon { get; set; }

		public string popup_icon_url { get; set; }
	}
}
