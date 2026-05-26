using HazelNet_Domain.Models;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace HazelNet_Infrastracture.DBContext;

public class ApplicationDbContext : DbContext
{
    public DbSet<Deck> Decks { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<ReviewHistory> ReviewHistory { get; set; }
    public DbSet<ReviewLog> ReviewLogs { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<FSRSParameters> FSRSParameters { get; set; }
    
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

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
            ?? "Production";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' was not found.");

        optionsBuilder.UseNpgsql(connectionString);
        
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
