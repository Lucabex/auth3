using Microsoft.AspNetCore;
using auth3.Data;
using auth3.Models;
using auth3.DTO;
using auth3.Records;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using auth3.Services;


namespace auth.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _service;
    private readonly IHttpClientFactory _client;
    public AuthController(AppDbContext context,JwtService service,IHttpClientFactory client)
    {
        _context=context;
        _service = service;
        _client = client;
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
        var userToken = _service.GenerateToken(user);

        return Ok(new
        {
            Message = "User Logged",
            Token = userToken,
            User = new
            {
                Id = user.Id,
                Name = user.Name
            }
        });
    }
    [HttpGet("daily")]
    public async Task<IActionResult> GetDailyPuzzle()
    {
       try{
            var client= _client.CreateClient();
        var url = "https://lichess.org/api/puzzle/daily";
        var response = await client.GetFromJsonAsync<DailyPuzzle>(url);
        if(response?.Puzzle.Solution == null || response?.Puzzle.Fen== null)
        {
            return StatusCode(503,"Service not available try again later");
        }
        return Ok(response);
        }catch(Exception ex)
        {
            return StatusCode(503,"Service not available try again later");
        }
        
    }

}
