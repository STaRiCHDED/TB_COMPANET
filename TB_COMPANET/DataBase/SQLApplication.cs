using Microsoft.EntityFrameworkCore;

namespace TB_COMPANET.DataBase;

public class SqlApplication : DbContext
{
    public SqlApplication()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(Configuration.SqlServerConfiguration);
    }
    
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Operation> Operations => Set<Operation>();
}