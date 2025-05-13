using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeSpace.Common.Dtos.User;
using Common.Repos;

namespace CodeSpace.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    public UsersController(AppDbContext ctx) => _db = ctx;

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll(
    [FromQuery] string? username,
    [FromQuery] string? email,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var q = _db.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(username))
            q = q.Where(u => u.Username.Contains(username));

        if (!string.IsNullOrWhiteSpace(email))
            q = q.Where(u => u.Email != null && u.Email.Contains(email));

        var total = await q.CountAsync();
        Response.Headers.Add("X-Total-Count", total.ToString());
        Response.Headers.Add("Access-Control-Expose-Headers", "X-Total-Count");

        return await q.Skip((page - 1) * pageSize)
                      .Take(pageSize)
                      .Select(u => new UserDto(u.Id, u.Username, u.Email, u.IsAdmin))
                      .ToListAsync();
    }


    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();

        user.Username = dto.Username;
        user.Email = dto.Email;
        user.IsAdmin = dto.IsAdmin;

        if (!string.IsNullOrWhiteSpace(dto.Password))
            user.Password = dto.Password;          // TODO: hash in a later task

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
