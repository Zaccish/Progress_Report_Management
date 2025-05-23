using Microsoft.AspNetCore.Mvc;
using Progress_Report_ManagementAPI.Models;
using ProgressReportAPI.Models;
using System.Security.Cryptography;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtHelper _jwtHelper;

    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _jwtHelper = new JwtHelper(config);
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] UserRegisterDto userDto)
    {
        var user = new User
        {
            Username = userDto.Username,
            MATID = userDto.Matid,
            PasswordHash = HashPassword(userDto.PasswordHash),
            Role = userDto.Role
        };

        _context.Users.Add(user);
        _context.SaveChanges();
        return Ok("User Registered");
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLoginDto login)
    {
        var user = _context.Users.FirstOrDefault(x => x.Username == login.Username);
        if (user == null || user.PasswordHash != HashPassword(login.PasswordHash))
            return Unauthorized("Invalid credentials");

        var token = _jwtHelper.GenerateToken(user.Username, user.Role, user.MATID.ToString());
        return Ok(new { Token = token });
    }

    private string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
