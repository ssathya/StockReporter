using Microsoft.EntityFrameworkCore;
using Models.Model;

namespace Models
{
    public class StockReporterContext : DbContext
    {
        #region Public Constructors

        public StockReporterContext(DbContextOptions options)
            : base(options)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public DbSet<CompanyDetail> CompanyDetails { get; set; }

        #endregion Public Properties

        #region Protected Methods

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CompanyDetail>()
                .HasIndex(cd => cd.Symbol)
                .HasName("idx_Symbol");

            base.OnModelCreating(modelBuilder);
        }

        #endregion Protected Methods
    }
}