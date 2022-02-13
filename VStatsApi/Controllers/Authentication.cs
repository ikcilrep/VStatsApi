using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    private VStatsContext _context;

    public Authentication(IConfiguration configuration, VStatsContext context)
    {
        _context = context;
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
    public async Task<IActionResult> Callback([FromQuery] string code)
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
        var data = HttpUtility.ParseQueryString(str);

        var accessToken = data["access_token"];

        var sessCode = Nanoid.Nanoid.Generate(size: 10);
        _context.AuthSessions.Add(new AuthSession
        {
            Code = sessCode,
            AccessToken = accessToken
        });
        await _context.SaveChangesAsync();

        return new ContentResult
        {
            ContentType = "text/html",
            StatusCode = (int)HttpStatusCode.OK,
            Content = $@"<head><title>Success</title></head>
<body style='display: flex; justify-content: center; align-items: center; font-family: sans-serif; text-align: center'>
<div>
<h1>Success</h1>
Copy and paste the following code: <br>
<pre style='padding: 5px; border-radius: 10px; background: #222; color: white; font-size: 1.5em'><code>{sessCode}</code></pre>
</div>
</body>"
        };
    }

    [HttpGet("finalize")]
    public async Task<LoginResult> Finalize(string code)
    {
        var sess = await _context.AuthSessions.FirstOrDefaultAsync(x => x.Code == code);
        if (sess == null) throw new BadHttpRequestException("");

        var user = await GetUser(sess.AccessToken);
        var token = new JwtSecurityTokenHandler().WriteToken(CreateJwt(user, sess.AccessToken));
        
        _context.AuthSessions.Remove(sess);

        var check = await _context.Users.FirstOrDefaultAsync(x => x.ID == user.Id);
        if (check == null)
        {
            _context.Users.Add(new User
            {
                ID = user.Id,
                Login = user.Login,
                Name = user.Name,
                AvatarURL = user.AvatarURL,
            });
        }
        else
        {
            check.Name = user.Name;
            check.Login = user.Login;
            check.AvatarURL = user.AvatarURL;
        }

        await _context.SaveChangesAsync();

        return new LoginResult
        {
            Token = token,
            User = user
        };
    }

    private JwtSecurityToken CreateJwt(GithubUser user, string accessToken)
    {
        var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSecretKey"]));

        return new JwtSecurityToken(issuer: _config["JwtValidIssuer"], audience: _config["JwtValidAudience"],
            signingCredentials: new SigningCredentials(issuerSigningKey, SecurityAlgorithms.HmacSha256),
            expires: DateTime.Now.AddMonths(2),
            claims: new[]
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
