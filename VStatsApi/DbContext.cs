using System.Configuration;
using VStatsApi.Models;
using Microsoft.EntityFrameworkCore;
using VStatsApi.Migrations;
using AuthSession = VStatsApi.Models.AuthSession;

namespace VStatsApi;

public class VStatsContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Stat> Stats { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<AuthSession> AuthSessions { get; set; }

    public VStatsContext(DbContextOptions<VStatsContext> options) : base(options)        
    {        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Stat>()
            .Navigation(c => c.User);
    }

}
