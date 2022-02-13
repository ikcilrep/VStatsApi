using System.ComponentModel.DataAnnotations;

namespace VStatsApi.Models;

public class AuthSession
{
    [Key]
    public string Code { get; set; }
    public string AccessToken { get; set; }
}
