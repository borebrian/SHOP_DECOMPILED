using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Lubes.DBContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SHOP.Models;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;

namespace SHOP.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		public class Total_cash_made
		{
			public string total { get; set; }
		}

		private readonly ApplicationDBContext _context;

		private readonly IWebHostEnvironment _webHostEnvironment;

		private readonly string today = DateTime.Now.ToString("dd/MM/yyyy");

		public HomeController(ApplicationDBContext context, IWebHostEnvironment webHostEnvironment)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
		}

		public IActionResult Index()
		{
			return View();
		}

		[Authorize(Roles = "2")]
		public IActionResult attendant()
		{
			List<sold_items> list_of_sold = _context.sold_items.Where((sold_items x) => x.DateTime == today).ToList();
			List<shop_items> list_of_brands = _context.Shop_items.ToList();
			List<join_sold_item> joinList = new List<join_sold_item>();
			var results = (from pd in list_of_sold
				join od in list_of_brands on pd.Item_id equals od.id
				select new { pd.DateTime, pd.quantity_sold, pd.Total_cash_made, od.Item_price, od.Item_name }).ToList();
			foreach (var item in results)
			{
				join_sold_item JoinObject = new join_sold_item();
				JoinObject.Item_name = item.Item_name;
				JoinObject.Item_price = item.Item_price;
				JoinObject.DateTime = item.DateTime;
				JoinObject.quantity_sold = item.quantity_sold;
				JoinObject.Total_cash_made = item.Total_cash_made;
				JoinObject.Item_price = item.Item_price;
				JoinObject.Item_name = item.Item_name;
				joinList.Add(JoinObject);
			}
			List<join_sold_item> JoinListToViewbag = joinList.ToList();
			base.ViewBag.JoinList = JoinListToViewbag;
			base.ViewBag.allBrands = _context.Shop_items.Where((shop_items x) => x.Quantity > 0f).ToList();
			base.ViewBag.allBrands_0 = _context.Shop_items.ToList();
			base.ViewBag.count_below = _context.Shop_items.Count((shop_items x) => x.Quantity <= 0f);
			base.ViewBag.to_restock = _context.Shop_items.Where((shop_items x) => x.Quantity <= 0f);
			base.ViewBag.count_all = _context.Shop_items.Count();
			base.ViewBag.sold = _context.sold_items.Where((sold_items x) => x.DateTime == today).Sum((sold_items x) => x.quantity_sold);
			base.ViewBag.shop_name = base.HttpContext.Session.GetString("shop_name");
			base.ViewBag.name = base.HttpContext.Session.GetString("Name");
			base.ViewBag.phone = base.HttpContext.Session.GetString("phone");
			Random r = new Random();
			base.ViewBag.Random = r.Next(1000000, 9999999);
			return View();
		}

		public IActionResult log_out()
		{
			base.HttpContext.Session.Clear();
			return Redirect("~/log_in/log_in");
		}

		[HttpPost]
		public IActionResult sell_Item(int id_finish, float quantity_sold, float submit_price, float Total_cash_made, string date)
		{
			sold_items check_if_exists = _context.sold_items.FirstOrDefault((sold_items x) => x.Item_id.ToString() == id_finish.ToString() && x.DateTime == date);
			shop_items shop_itemss = _context.Shop_items.FirstOrDefault((shop_items x) => x.id == id_finish);
			if (check_if_exists != null)
			{
				float initial_quantity = shop_itemss.Quantity;
				float initial_sold_quantity = check_if_exists.quantity_sold;
				if (initial_quantity <= 0f)
				{
					Total_cash_made person3 = new Total_cash_made
					{
						total = "Unable to sell the item.Stock running low"
					};
					return Json(person3);
				}
				float new_quantity = initial_quantity + quantity_sold;
				float new_sold_quantity = initial_sold_quantity + quantity_sold;
				float initial_total_cash = check_if_exists.Total_cash_made;
				float new_total_cash = initial_total_cash + Total_cash_made;
				check_if_exists.DateTime = date;
				check_if_exists.Total_cash_made = new_total_cash;
				check_if_exists.quantity_sold = new_sold_quantity;
				_context.Entry(check_if_exists).State = EntityState.Modified;
				_context.SaveChanges();
				shop_items change_r2 = _context.Shop_items.FirstOrDefault((shop_items x) => x.id == id_finish);
				float initial_q2 = change_r2.Quantity;
				float new_Q2 = (change_r2.Quantity = initial_q2 - quantity_sold);
				_context.Entry(change_r2).State = EntityState.Modified;
				_context.SaveChanges();
				Total_cash_made person2 = new Total_cash_made
				{
					total = new_sold_quantity.ToString()
				};
				return Json(person2);
			}
			shop_items change_r = _context.Shop_items.FirstOrDefault((shop_items x) => x.id == id_finish);
			float initial_q = change_r.Quantity;
			float new_Q = (change_r.Quantity = initial_q - quantity_sold);
			_context.Entry(change_r).State = EntityState.Modified;
			_context.SaveChanges();
			Total_cash_made person = new Total_cash_made
			{
				total = "Sales have been done successfully and receipt downloaded and stored in your local machine"
			};
			sold_items x2 = new sold_items
			{
				Item_id = id_finish,
				quantity_sold = quantity_sold,
				Total_cash_made = Total_cash_made,
				DateTime = date
			};
			_context.Add(x2);
			_context.SaveChanges();
			return Json(person);
		}

		public IActionResult delete_item(int id)
		{
			shop_items itemToRemove = _context.Shop_items.SingleOrDefault((shop_items x) => x.id == id);
			string item_name = itemToRemove.Item_name;
			if (itemToRemove != null)
			{
				_context.Shop_items.Remove(itemToRemove);
				_context.SaveChanges();
				base.TempData["popup"] = "1";
				base.TempData["message"] = "You have successfully removed :" + item_name + " from the system";
			}
			else
			{
				base.TempData["popup"] = "2";
				base.TempData["message"] = "Error! item does not exists!";
			}
			return Redirect("~/home/admin");
		}

		public IActionResult restock(int id, int ammount)
		{
			shop_items rest = _context.Shop_items.FirstOrDefault((shop_items x) => x.id == id);
			float initialStock = rest.Quantity;
			float new_stock = (rest.Quantity = initialStock + (float)ammount);
			_context.Entry(rest).State = EntityState.Modified;
			_context.SaveChanges();
			string date = DateTime.Now.ToString();
			Restock_history insert_new = new Restock_history
			{
				Item_id = id,
				Date_restock = date,
				new_quanity = new_stock,
				Prev_quantity = initialStock,
				quantity = ammount
			};
			_context.Add(insert_new);
			_context.SaveChanges();
			base.TempData["popup"] = "1";
			base.TempData["message"] = "You have successfully added stock to " + rest.Item_name + " from: " + initialStock + " to:" + new_stock;
			return Redirect("~/home/admin");
		}

		public IActionResult change_price(int change_price_id, int new_price)
		{
			shop_items rest = _context.Shop_items.FirstOrDefault((shop_items x) => x.id == change_price_id);
			float initial_price = rest.Item_price;
			rest.Item_price = new_price;
			_context.Entry(rest).State = EntityState.Modified;
			_context.SaveChanges();
			string date = DateTime.Now.ToString();
			base.TempData["popup"] = "1";
			base.TempData["message"] = "You have successfully changed price of:" + rest.Item_name + " from: " + initial_price + " to:" + new_price;
			return Redirect("~/home/admin");
		}

		public IActionResult pdf_test()
		{
			return View();
		}

		public IActionResult generatePDF()
		{
			PdfDocument doc = new PdfDocument();
			PdfPage page = doc.Pages.Add();
			PdfGrid pdfGrid = new PdfGrid();
			PdfGraphics graphics = page.Graphics;
			FileStream imageStream = new FileStream("wwwroot/images/logo2.png", FileMode.Open, FileAccess.Read);
			PdfBitmap image = new PdfBitmap(imageStream);
			graphics.DrawImage(image, 0f, 0f);
			PdfGridCellStyle headerStyle = new PdfGridCellStyle();
			headerStyle.Borders.All = new PdfPen(new PdfColor(126, 151, 173));
			headerStyle.BackgroundBrush = new PdfSolidBrush(new PdfColor(126, 151, 173));
			headerStyle.TextBrush = PdfBrushes.White;
			headerStyle.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 14f, PdfFontStyle.Regular);
			List<shop_items> list_all = _context.Shop_items.ToList();
			IEnumerable<object> dataTable = (IEnumerable<object>)(pdfGrid.DataSource = list_all);
			pdfGrid.Draw(page, new PointF(10f, 10f));
			MemoryStream stream = new MemoryStream();
			doc.Save(stream);
			stream.Position = 0L;
			doc.Close(completely: true);
			string contentType = "application/pdf";
			string fileName = "Output.pdf";
			return File(stream, contentType, fileName);
		}

		[HttpPost]
		public JsonResult setTempData(string name)
		{
			Total_cash_made person = new Total_cash_made
			{
				total = name
			};
			return Json(person);
		}

		[HttpPost]
		public JsonResult AjaxMethod(string name)
		{
			Total_cash_made person = new Total_cash_made
			{
				total = name
			};
			return Json(person);
		}

		[Authorize(Roles = "1")]
		public IActionResult admin(log_in log, [Optional] string date)
		{
			List<sold_items> list_of_sold = _context.sold_items.Where((sold_items x) => x.DateTime == today).ToList();
			List<shop_items> list_of_brandss = _context.Shop_items.ToList();
			List<join_sold_item> joinList2 = new List<join_sold_item>();
			var results2 = (from pd in list_of_sold
				join od in list_of_brandss on pd.Item_id equals od.id
				select new { pd.DateTime, pd.quantity_sold, pd.Total_cash_made, od.Item_price, od.Item_name }).ToList();
			foreach (var item3 in results2)
			{
				join_sold_item JoinObject2 = new join_sold_item();
				JoinObject2.Item_name = item3.Item_name;
				JoinObject2.Item_price = item3.Item_price;
				JoinObject2.DateTime = item3.DateTime;
				JoinObject2.quantity_sold = item3.quantity_sold;
				JoinObject2.Total_cash_made = item3.Total_cash_made;
				JoinObject2.Item_price = item3.Item_price;
				JoinObject2.Item_name = item3.Item_name;
				joinList2.Add(JoinObject2);
			}
			List<join_sold_item> JoinListToViewbag2 = joinList2.ToList();
			base.ViewBag.JoinList1 = JoinListToViewbag2;
			if (date == null)
			{
				List<sold_items> list_of_sold_second = _context.sold_items.ToList();
				List<shop_items> list_of_brands_second = _context.Shop_items.ToList();
				List<join_sold_item> joinList_second = new List<join_sold_item>();
				var results_second = (from pd in list_of_sold_second
					join od in list_of_brands_second on pd.Item_id equals od.id
					select new { pd.DateTime, pd.quantity_sold, pd.Total_cash_made, od.Item_price, od.Item_name }).ToList();
				foreach (var item5 in results_second)
				{
					join_sold_item JoinObject_second = new join_sold_item();
					JoinObject_second.Item_name = item5.Item_name;
					JoinObject_second.Item_price = item5.Item_price;
					JoinObject_second.DateTime = item5.DateTime;
					JoinObject_second.quantity_sold = item5.quantity_sold;
					JoinObject_second.Total_cash_made = item5.Total_cash_made;
					JoinObject_second.Item_price = item5.Item_price;
					JoinObject_second.Item_name = item5.Item_name;
					joinList_second.Add(JoinObject_second);
				}
				List<join_sold_item> JoinListToViewbag_second = joinList_second.ToList();
				base.ViewBag.JoinList12 = JoinListToViewbag_second;
			}
			else
			{
				string day = DateTime.Parse(date).ToString("dd/MM/yyyy");
				List<sold_items> list_of_sold_third = _context.sold_items.Where((sold_items x) => x.DateTime == day).ToList();
				List<shop_items> list_of_brands_third = _context.Shop_items.ToList();
				List<join_sold_ite_filtered> joinList_third = new List<join_sold_ite_filtered>();
				var results_third = (from pd in list_of_sold_third
					join od in list_of_brands_third on pd.Item_id equals od.id
					select new { pd.DateTime, pd.quantity_sold, pd.Total_cash_made, od.Item_price, od.Item_name }).ToList();
				foreach (var item4 in results_third)
				{
					join_sold_ite_filtered JoinObject_third = new join_sold_ite_filtered();
					JoinObject_third.Item_name = item4.Item_name;
					JoinObject_third.DateTime = item4.DateTime;
					JoinObject_third.quantity_sold = item4.quantity_sold;
					JoinObject_third.Total_cash_made = item4.Total_cash_made;
					JoinObject_third.Item_price = item4.Item_price;
					JoinObject_third.Item_name = item4.Item_name;
					joinList_third.Add(JoinObject_third);
				}
				List<join_sold_ite_filtered> JoinListToViewbag_third = joinList_third.ToList();
				int count = joinList_third.Count();
				float sum_of_cash = joinList_third.Sum((join_sold_ite_filtered x) => x.Total_cash_made);
				base.TempData["popup"] = 4;
				base.TempData["message"] = count + " records found totaling to Ksh. " + sum_of_cash;
				base.TempData["total"] = sum_of_cash;
				base.ViewBag.JoinList_general_third = JoinListToViewbag_third;
			}
			base.ViewBag.allBrands = _context.Shop_items.Where((shop_items x) => x.Quantity > 0f).ToList();
			base.ViewBag.allBrands_0 = _context.Shop_items.ToList();
			base.ViewBag.count_below = _context.Shop_items.Count((shop_items x) => x.Quantity <= 0f);
			base.ViewBag.to_restock = _context.Shop_items.Where((shop_items x) => x.Quantity <= 0f);
			base.ViewBag.count_all = _context.Shop_items.Count();
			base.ViewBag.sold = _context.sold_items.Where((sold_items x) => x.DateTime == today).Sum((sold_items x) => x.quantity_sold);
			base.ViewBag.sold_general = _context.sold_items;
			base.ViewBag.shop_name = base.HttpContext.Session.GetString("shop_name");
			base.ViewBag.name = base.HttpContext.Session.GetString("Name");
			List<Restock_history> list_of_restocked = _context.Restock_history.ToList();
			List<shop_items> list_of_brands = _context.Shop_items.ToList();
			List<join_tables> joinList = new List<join_tables>();
			DbSet<shop_items> shop_items = _context.Shop_items;
			DbSet<Restock_history> restock_item = _context.Restock_history;
			var results = (from pd in list_of_restocked
				join od in list_of_brands on pd.Item_id equals od.id
				select new { pd.Date_restock, pd.Prev_quantity, pd.new_quanity, od.Item_name, od.Item_price, od.Quantity, od.id }).ToList();
			foreach (var item2 in results)
			{
				join_tables JoinObject = new join_tables();
				JoinObject.Item_id = item2.id.ToString();
				JoinObject.Item_name = item2.Item_name;
				JoinObject.Item_price = item2.Item_price;
				JoinObject.new_quanity = item2.new_quanity;
				JoinObject.Prev_quantity = item2.Prev_quantity;
				JoinObject.quantity = item2.Quantity;
				JoinObject.Date_restock = item2.Date_restock;
				joinList.Add(JoinObject);
			}
			List<join_tables> JoinListToViewbag = joinList.ToList();
			base.ViewBag.JoinList = JoinListToViewbag;
			base.ViewBag.allBrands = _context.Shop_items.ToList();
			base.ViewBag.all_attendants = _context.Log_in.Where((log_in item) => item.strRole == 2).ToList();
			int count_brand = _context.Shop_items.Count();
			base.ViewBag.count_all = count_brand;
			int count_below = _context.Shop_items.Where((shop_items x) => x.Quantity < 5f).Count();
			base.ViewBag.count_below = count_below;
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		public IActionResult delete_attwndant(string id)
		{
			log_in itemToRemove = _context.Log_in.SingleOrDefault((log_in x) => x.id.ToString() == id);
			string nameOfVictim = itemToRemove.Full_name;
			if (itemToRemove != null)
			{
				_context.Log_in.Remove(itemToRemove);
				_context.SaveChanges();
			}
			base.TempData["popup"] = "3";
			base.TempData["message"] = nameOfVictim + " has been successfully removed from the system!";
			base.TempData["total"] = nameOfVictim + " has been successfully removed from the system!";
			return Redirect("~/home/admin");
		}

		public IActionResult add_new_attendant(string Full_name, string Phone)
		{
			log_in check_if_item_exists1 = _context.Log_in.SingleOrDefault((log_in x) => x.Phone == Phone);
			if (check_if_item_exists1 != null)
			{
				base.TempData["popup"] = "2";
				base.TempData["message"] = Full_name + " already exists in the system!";
				return Redirect("~/home/admin");
			}
			log_in shop = _context.Log_in.FirstOrDefault((log_in x) => x.strRole == 1);
			log_in add_new_attendant = new log_in
			{
				Full_name = Full_name,
				Phone = Phone,
				Password = Phone,
				Shop_name = shop.Shop_name,
				strRole = 2
			};
			_context.Add(add_new_attendant);
			_context.SaveChanges();
			base.TempData["popup"] = "1";
			base.TempData["message"] = Full_name + " successfully added to the list!";
			return Redirect("~/home/admin");
		}

		public IActionResult add_item(string Item_name, float Item_price, int Quantity, string date)
		{
			shop_items check_if_item_exists = _context.Shop_items.SingleOrDefault((shop_items x) => x.Item_name == Item_name);
			if (check_if_item_exists == null)
			{
				shop_items add_new_product = new shop_items
				{
					Item_name = Item_name,
					Item_price = Item_price,
					Quantity = Quantity,
					DateTime = date
				};
				_context.Add(add_new_product);
				_context.SaveChanges();
				base.TempData["popup"] = "5";
				base.TempData["message"] = Item_name + " successfully added to the list!";
			}
			else
			{
				base.TempData["popup"] = "5";
				base.TempData["message"] = Item_name + " already exists in the system!";
			}
			return Redirect("~/home/admin");
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel
			{
				RequestId = (Activity.Current?.Id ?? base.HttpContext.TraceIdentifier)
			});
		}

		public IActionResult error_page()
		{
			return View();
		}
	}
}
