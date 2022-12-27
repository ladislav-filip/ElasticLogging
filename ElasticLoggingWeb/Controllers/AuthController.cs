using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ElasticLoggingWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ElasticLoggingWeb.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IConfiguration _configuration;

    public AuthController(ILogger<AuthController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Sign in endpoint. User username "host" and password "host"
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="BadHttpRequestException"></exception>
    [AllowAnonymous]
    [HttpPost("sign-in")]
    public IActionResult SignIn([FromBody]UserSignIn model)
    {
        if (model.Username != "host" && model.Password != "host")
        {
            throw new BadHttpRequestException("Username or password are invalid");
        }
        
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, model.Username),
                new Claim(JwtRegisteredClaimNames.Email, model.Username),
                new Claim(JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(5),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        tokenHandler.WriteToken(token);
        var stringToken = tokenHandler.WriteToken(token);
        _logger.LogInformation("Sign in user: {Username}", model.Username);
        return Ok(stringToken);
    }
}