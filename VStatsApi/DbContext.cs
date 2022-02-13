using System.Configuration;
using VStatsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace VStatsApi;

public class VStatsContext : DbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(Configuration.GetConnectionString("VStatsContext"));
}
