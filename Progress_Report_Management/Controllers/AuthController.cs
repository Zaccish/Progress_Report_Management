using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgressReportSystem.API.Data;
using ProgressReportSystem.API.DTOs;
using ProgressReportSystem.API.Helpers;
using ProgressReportSystem.API.Models;
using ProgressReportSystem.API.Services;

namespace ProgressReportSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private readonly IActivityService _activityService;

    public AuthController(AppDbContext db, IConfiguration config, IActivityService activityService)
    {
        _db = db;
        _config = config;
        _activityService = activityService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO dto)
    {
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("User already exists");

        if (dto.Role != "Student" && dto.Role != "Admin")
            return BadRequest("Invalid role. Must be either 'Student' or 'Admin'.");

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            MatricId = dto.MatricId,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role,
            IsApprovedAdmin = dto.Role == "Admin" ? false : false
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        string ip = IPHelper.GetClientIp(HttpContext);
        await _activityService.LogActivityAsync(user.Id, "Register", $"User {user.FullName} registered as {dto.Role}", ip);

        var message = dto.Role == "Admin"
            ? "Registration successful. Admin account is pending SuperAdmin approval."
            : "Registration successful.";

        return Ok(message);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDTO>> Login(LoginDTO dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        string ip = IPHelper.GetClientIp(HttpContext);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            await _activityService.LogFailedAttemptAsync(dto.Email, "Invalid login credentials", ip);
            return Unauthorized("Invalid credentials");
        }

        await _activityService.LogActivityAsync(user.Id, "Login", $"User {user.FullName} logged in", ip);

        var token = JwtHelper.GenerateJwtToken(user, _config);

        return Ok(new AuthResponseDTO
        {
            Token = token,
            Role = user.Role,
            FullName = user.FullName
        });
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpGet("superadmin/dashboard")]
    public IActionResult SuperAdminDashboard()
    {
        return Ok("Welcome SuperAdmin");
    }

    [HttpPost("request-otp")]
    public async Task<IActionResult> RequestOtp([FromBody] OtpRequestDTO dto, [FromServices] IEmailService emailService)
    {
        if (!dto.Email.EndsWith("@gmail.com"))
            return BadRequest("Only Gmail addresses are allowed.");

        var recentOtps = await _db.Otps
            .Where(o => o.Email == dto.Email && o.CreatedAt >= DateTime.UtcNow.AddMinutes(-15))
            .CountAsync();

        if (recentOtps >= 3)
            return BadRequest("You've requested OTP too many times. Please wait a few minutes and try again.");

        var otpCode = new Random().Next(100000, 999999).ToString();

        var otp = new OneTimePassword
        {
            Email = dto.Email,
            Code = otpCode,
            Expiry = DateTime.UtcNow.AddMinutes(5),
            CreatedAt = DateTime.UtcNow
        };

        _db.Otps.Add(otp);
        await _db.SaveChangesAsync();

        await emailService.SendEmailAsync(
            dto.Email,
            "🔐 Your OTP Code",
            $"<p>Hello,</p><p>Your verification code is <b>{otpCode}</b>. It expires in 5 minutes.</p>"
        );

        string ip = IPHelper.GetClientIp(HttpContext);
        // ✅ Log anonymous OTP request without UserId
        await _activityService.LogActivityAsync(0, "OTP Requested", $"OTP requested for {dto.Email}", ip);


        return Ok("OTP sent to your Gmail.");
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyDTO dto)
    {
        var record = await _db.Otps
            .Where(o => o.Email == dto.Email && o.Code == dto.Otp)
            .OrderByDescending(o => o.Expiry)
            .FirstOrDefaultAsync();

        if (record == null || record.Expiry < DateTime.UtcNow)
            return BadRequest("Invalid or expired OTP.");

        _db.Otps.Remove(record);
        await _db.SaveChangesAsync();

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        bool isNew = false;

        if (user == null)
        {
            user = new User
            {
                Email = dto.Email,
                FullName = dto.FullName,
                MatricId = dto.MatricId,
                Role = "Student",
                PasswordHash = string.Empty,
                IsEmailConfirmed = true
            };

            _db.Users.Add(user);
            isNew = true;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(user.FullName)) user.FullName = dto.FullName;
            if (string.IsNullOrWhiteSpace(user.MatricId)) user.MatricId = dto.MatricId;
            user.IsEmailConfirmed = true;
        }

        await _db.SaveChangesAsync();

        string ip = IPHelper.GetClientIp(HttpContext);
        await _activityService.LogActivityAsync(user.Id, "OTP Login", isNew ? "New user created via OTP" : "Logged in using Gmail OTP", ip);

        var token = JwtHelper.GenerateJwtToken(user, _config);

        return Ok(new AuthResponseDTO
        {
            Token = token,
            Role = user.Role,
            FullName = user.FullName
        });
    }
}
