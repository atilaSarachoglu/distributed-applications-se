using CodeSpace.Common.Dtos.Feed;

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
    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateReply(int id, [FromBody] UpdateReplyRequest dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var reply = await _db.Replies.FindAsync(id);
        if (reply is null) return NotFound();

        var isAdmin = User.HasClaim("isAdmin", "true");
        if (reply.UserId != userId && !isAdmin) return Forbid();

        reply.Content = dto.Content;
        await _db.SaveChangesAsync();
        return NoContent();
    }

}
