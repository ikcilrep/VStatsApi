namespace VStatsApi.Models;

public class Stat
{
    public int ID { get; set; }
    public int UserID { get; set; }
    public User? User { get; set; }
    public int ProjectID { get; set; }
    public Project? Project { get; set; }
    public string Language { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public int LinesOfCode { get; set; }
    public int CharacterCount { get; set; }
    public int FileCount { get; set; }
    public int ProblemCount { get; set; }
}


public class LeaderboardStat
{
    public int UserID { get; set; }
    public User User { get; set; }
    public int ProjectID { get; set; }
    public string Language { get; set; }
    
    public int LinesOfCode { get; set; }
    public int CharacterCount { get; set; }
    public int FileCount { get; set; }
    public int ProblemCount { get; set; }
}
