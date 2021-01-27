using System.Linq;
using System.Threading.Tasks;
using Lubes.DBContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SHOP.Models;
using SHOP_DECOMPILED;

namespace SHOP.Controllers
{
	public class log_inController : Controller
	{
		private readonly ApplicationDBContext _context;

		public log_inController(ApplicationDBContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			return View(await _context.Log_in.ToListAsync());
		}

		public async Task<IActionResult> Details(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			log_in log_in = await _context.Log_in.FirstOrDefaultAsync((log_in m) => (int?)m.id == id);
			if (log_in == null)
			{
				return NotFound();
			}
			return View(log_in);
		}

		public IActionResult Create()
		{
			log_in user = _context.Log_in.FirstOrDefault();
			if (user == null)
			{
				return View();
			}
			return Redirect("~/log_in/log_in");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind(new string[] { "Full_name,Shop_name,Phone,Password,strRole,id" })] log_in log_in)
		{
			if (base.ModelState.IsValid)
			{
				_context.Add(log_in);
				await _context.SaveChangesAsync();
				return RedirectToAction("Log_in");
			}
			return RedirectToAction("log_in");
		}

		public IActionResult Log_in(Users users)
		{
			base.HttpContext.Session.Clear();
			return View();
		}

		[HttpPost]
		public IActionResult Log_in_user(Users users)
		{
			string pass = users.Password;
			TokenProvider TokenProviderr = new TokenProvider(_context);
			string userToken = TokenProviderr.LoginUser(users.Phone.ToString(), users.Password);
			if (userToken == null)
			{
				base.TempData["Error"] = "Invalid login credentials";
				return Redirect("~/log_in/Log_in");
			}
			base.HttpContext.Session.SetString("JWToken", userToken);
			log_in user_id = _context.Log_in.Where((log_in x) => x.Phone == users.Phone).SingleOrDefault();
			if (user_id.strRole.ToString() == "1")
			{
				base.HttpContext.Session.SetString("roles", user_id.strRole.ToString());
				base.HttpContext.Session.SetString("Name", user_id.Full_name);
				base.HttpContext.Session.SetString("shop_name", user_id.Shop_name);
				log_in constants2 = _context.Log_in.FirstOrDefault((log_in x) => x.strRole == 1);
				base.HttpContext.Session.SetString("phone", constants2.Phone);
				return Redirect("~/home/admin");
			}
			base.HttpContext.Session.SetString("roles", user_id.strRole.ToString());
			base.HttpContext.Session.SetString("Name", user_id.Full_name);
			base.HttpContext.Session.SetString("shop_name", user_id.Shop_name);
			log_in constants = _context.Log_in.FirstOrDefault((log_in x) => x.strRole == 1);
			base.HttpContext.Session.SetString("phone", constants.Phone);
			return Redirect("~/home/attendant");
		}

		public async Task<IActionResult> Edit(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			log_in log_in = await _context.Log_in.FindAsync(id);
			if (log_in == null)
			{
				return NotFound();
			}
			return View(log_in);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind(new string[] { "Full_name,Phone,Password,strRole,id" })] log_in log_in)
		{
			if (id != log_in.id)
			{
				return NotFound();
			}
			if (base.ModelState.IsValid)
			{
				try
				{
					_context.Update(log_in);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!log_inExists(log_in.id))
					{
						return NotFound();
					}
					throw;
				}
				return RedirectToAction("Index");
			}
			return View(log_in);
		}

		public async Task<IActionResult> Delete(int? id)
		{
			if (!id.HasValue)
			{
				return NotFound();
			}
			log_in log_in = await _context.Log_in.FirstOrDefaultAsync((log_in m) => (int?)m.id == id);
			if (log_in == null)
			{
				return NotFound();
			}
			return View(log_in);
		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			log_in log_in = await _context.Log_in.FindAsync(id);
			_context.Log_in.Remove(log_in);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}

		private bool log_inExists(int id)
		{
			return _context.Log_in.Any((log_in e) => e.id == id);
		}
	}
}
