using Microsoft.AspNetCore;
using auth3.Data;
using auth3.Models;
using auth3.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace auth.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    public AuthController(AppDbContext context)
    {
        _context=context;
    }
    [HttpPost("register")]
    public async Task<IActionResult> RegUser(RegDto dto)
    {
        if(await _context.Users.AnyAsync(u=> u.Name.ToLower() == dto.Name.ToLower()))
        {
            return BadRequest("user already in use");
        }
        var user = new User
        {
            Name = dto.Name,
            HashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password),
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok ("User registered");
        
    }
    [HttpPost("log")]
    public async Task<IActionResult> Log(LogDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u=> u.Name == dto.Name);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password , user.HashedPassword))
        {
            return BadRequest("User name or password not correct");
        }
        return Ok("User logged in");
    }

}
