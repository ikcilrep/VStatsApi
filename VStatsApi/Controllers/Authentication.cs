using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using VStatsApi.Models;

namespace VStatsApi.Controllers;

public class LoginResult
{
    public GithubUser User { get; set; }
    public string Token { get; set; }
}



[ApiController, Route("auth")]
public class Authentication : Controller
{
    private IConfiguration _config;
    private string _githubURL;

    public Authentication(IConfiguration configuration)
    {
        _config = configuration;
        _githubURL =
            $"https://github.com/login/oauth/authorize?client_id={_config["GithubClientID"]}&scope={_config["GithubScope"]}&redirect_uri={_config["GithubRedirectURL"]}";
    }

    [HttpGet]
    public IActionResult Index()
    {
        return Redirect(_githubURL);
    }

    [HttpGet("callback")]
    public async Task<LoginResult> Callback([FromQuery] string code)
    {
        using var client = new HttpClient();
        var res = await client.PostAsync("https://github.com/login/oauth/access_token", new FormUrlEncodedContent(
            new List<KeyValuePair<string, string>>
            {
                new("client_id", _config["GithubClientID"]),
                new("client_secret", _config["GithubClientSecret"]),
                new("code", code),
                new("redirect_uri", _config["GithubRedirectURL"])
            }));
        res.EnsureSuccessStatusCode();
        var str = await res.Content.ReadAsStringAsync();
        // Console.WriteLine(str);
        
        var data = HttpUtility.ParseQueryString(str);
        var accessToken = data["access_token"];
        var user = await GetUser(accessToken);

        var token = new JwtSecurityTokenHandler().WriteToken(CreateJwt(user, accessToken));
        return new LoginResult
        {
            Token = token,
            User = user,
        };
    }

    private JwtSecurityToken CreateJwt(GithubUser user, string accessToken)
    {
        var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSecretKey"]));

        return new JwtSecurityToken(issuer: _config["JwtValidIssuer"], audience: _config["JwtValidAudience"],
            signingCredentials: new SigningCredentials(issuerSigningKey, SecurityAlgorithms.HmacSha256),
            expires: DateTime.Now.AddMonths(2),
            claims: new []
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Authentication, accessToken)
            });
    }

    private async Task<GithubUser> GetUser(string accessToken)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", accessToken);
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("aspnet", "1.0"));
        var res = await client.GetAsync("https://api.github.com/user");
        var str = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<GithubUser>(str);
    }
}
