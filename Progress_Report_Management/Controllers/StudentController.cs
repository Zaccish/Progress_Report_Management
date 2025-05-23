using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProgressReportAPI.Models;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class StudentController : ControllerBase
{
    private readonly AppDbContext _context;

    public StudentController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_context.Students.ToList());

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult AddStudent([FromBody] Student student)
    {
        _context.Students.Add(student);
        _context.SaveChanges();
        return Ok("Student added.");
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteStudent(int id)
    {
        var student = _context.Students.Find(id);
        if (student == null) return NotFound();

        _context.Students.Remove(student);
        _context.SaveChanges();
        return Ok("Deleted");
    }

    [HttpGet("{id}/Download")]
    [Authorize(Roles = "Admin,Student")]
    public async Task<IActionResult> DownloadStudentFile(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
            return NotFound("Student not found.");

        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userRole == "Student" && userIdClaim != null && student.Student_id.ToString() != userIdClaim)
        {
            return Forbid("Students can only download their own reports.");
        }

        if (string.IsNullOrEmpty(student.File_path) || !System.IO.File.Exists(student.File_path))
            return NotFound("File not found on disk.");

        var fileBytes = await System.IO.File.ReadAllBytesAsync(student.File_path);
        var fileName = student.File_name ?? "StudentReport.pdf";

        return File(fileBytes, "application/pdf", fileName);
    }





}
