using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using VStatsApi.Models;

namespace VStatsApi.Controllers;

[Authorize]
[ApiController, Route("stats")]
public class StatsController : Controller
{
    private VStatsContext _context;
    public StatsController(VStatsContext context)
    {
        _context = context;
    }
    
    [HttpGet("")]
    public async Task<List<Stat>> GetStats(int? projectID, int? userID, string? language)
    {
        return await _context.Stats
            .Where(x => x.ProjectID == (projectID ?? -1) || x.UserID == (userID ?? -1) || x.Language == language)
            .ToListAsync();
    }

    [HttpPost("")]
    public async Task<Stat> PostStats(Stat stat)
    {
        stat.UserID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        _context.Stats.Add(stat);
        await _context.SaveChangesAsync();
        return stat;
    }
}
