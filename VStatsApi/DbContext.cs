using System.Configuration;
using VStatsApi.Models;
using Microsoft.EntityFrameworkCore;

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
}
