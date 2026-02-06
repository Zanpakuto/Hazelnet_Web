using HazelNet_Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HazelNet_Infrastracture.DBContext;

public class ApplicationDbContext : DbContext
{
    public DbSet<Deck> Decks { get; set; }
    public DbSet<Card> Cards { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        :base(options) {}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=HazelNetDb;Username=postgres;Password=Password");
        
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
