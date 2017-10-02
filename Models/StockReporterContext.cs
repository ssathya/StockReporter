using Microsoft.EntityFrameworkCore;
using Models.Model;

namespace Models
{
    public class StockReporterContext : DbContext
    {
        public StockReporterContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<CompanyDetail> CompanyDetails { get; set; }
    }
}