using Mango.Services.OrderApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.OrderApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }

    }
}
