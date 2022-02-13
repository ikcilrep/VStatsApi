using System.Configuration;
using VStatsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace VStatsApi;

public class VStatsContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Stat> Stats { get; set; }

    public VStatsContext(DbContextOptions<VStatsContext> options) : base(options)        
    {        
    }
}
