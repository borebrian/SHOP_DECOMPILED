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
		public DbSet<Creditors_account> Creditors_account { get; set; }

		public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
			: base(options)
		{
		}
	}
}
