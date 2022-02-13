using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using VStatsApi.Models;

namespace VStatsApi.Controllers;

public class LeaderboardRecord
{
    public User User { get; set; }
    public Stat Stat { get; set; }
}

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

    [HttpGet("top")]
    public async Task<List<Stat>> GetTop(string language, int limit)
    {
        var query = await _context.Stats.FromSqlRaw(@"
	select max(y.""UserID"") as ""UserID"", max(y.""ID"") as ""ID"", sum(y.""LinesOfCode"") as ""LinesOfCode"", 
	       sum(y.""CharacterCount"") as ""CharacterCount"", sum(y.""ProblemCount"") as ""ProblemCount"", sum(y.""FileCount"") as ""FileCount"",
	       max(y.""CreatedAt"") as ""CreatedAt"", max(y.""Language"") as ""Language"", max(y.""ProjectID"") as ""ProjectID""
	from (select *
		from (select distinct on (""ProjectID"") * from ""Stats"" order by ""ProjectID"", ""LinesOfCode"" desc) as x
		where ""Language"" = {0}) y
	group by ""UserID""
    order by ""LinesOfCode"" desc
    limit {1}
", language, limit).Include(x => x.User).ToListAsync();
        return query;
    }

    [HttpPost("project")]
    public async Task<Project> CreateProject()
    {
        var p = new Project
        {
            UserID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))
        };
        _context.Projects.Add(p);
        await _context.SaveChangesAsync();
        return p;
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
