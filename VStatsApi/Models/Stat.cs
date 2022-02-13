﻿namespace VStatsApi.Models;

public class Stat
{
    public int ID { get; set; }
    public int UserID { get; set; }
    public int ProjectID { get; set; }
    public string Language { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public int LinesOfCode { get; set; }
    public int CharacterCount { get; set; }
    public int FileCount { get; set; }
    public int ProblemCount { get; set; }
}
