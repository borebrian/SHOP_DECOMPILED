using Microsoft.EntityFrameworkCore;
using SHOP.Models;

namespace Lubes.DBContext
{
	public class ApplicationDBContext : DbContext
	{
		public DbSet<shop_items> Shop_items { get; set; }

		public DbSet<Restock_history> Restock_history { get; set; }

		public DbSet<log_in> Log_in { get; set; }

		public DbSet<sold_items> sold_items { get; set; }
		public DbSet<Creditors_account> Creditors { get; set; }
		public DbSet<Credits> Credits { get; set; }
		public DbSet<Payment_history> Payment_history { get; set; }
		public DbSet<Expiries> Expiries { get; set; }
		public DbSet<Suppliers> Supliers { get; set; }
		public DbSet<Settings> Settings { get; set; }

		public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
			: base(options)
		{
		}
	}
}
