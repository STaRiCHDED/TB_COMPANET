using Microsoft.EntityFrameworkCore;

namespace TB_COMPANET.DataBase;

public class SQLApplication : DbContext
{
    public SQLApplication()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Server=DESKTOP-9I6MJKT\SQLEXPRESS;Database=DB_Company;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");
    }
    
    public DbSet<Company> Comanies => Set<Company>();
    public DbSet<Operation> Operations => Set<Operation>();
}