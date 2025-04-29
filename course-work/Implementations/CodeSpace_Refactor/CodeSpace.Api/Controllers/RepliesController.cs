using Common.Repos;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace CodeSpace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RepliesController : ControllerBase
{
    private readonly AppDbContext _db;
    public RepliesController(AppDbContext db) => _db = db;

    [Authorize]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var reply = _db.Replies.SingleOrDefault(r => r.Id == id);
        if (reply is null) return NotFound();

        var me = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var isAdmin = bool.Parse(User.FindFirst("isAdmin")!.Value);

        if (!(isAdmin || reply.UserId == me)) return Forbid();

        _db.Replies.Remove(reply);
        _db.SaveChanges();
        return NoContent();
    }
}
