using Common.Entities;
using Common.Repos;
using CodeSpace.Common.ApiDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeSpace.Models.FeedInteractions;
using System.Security.Claims;
using CodeSpace.Common.Dtos.Feed;

namespace CodeSpace.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedController : ControllerBase
    {
        private readonly AppDbContext _db;

        public FeedController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IEnumerable<PostDto>> GetFeed(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string orderBy = "createdAt",            // createdAt|likes
            [FromQuery] string? searchTitle = null,
            [FromQuery] string? searchContent = null)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 20 : pageSize;

            var isAdmin = User.HasClaim("isAdmin", "true");
            var me = User.Identity!.IsAuthenticated
                            ? int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value)
                            : 0;

            /* ---------- base query ---------- */
            var q = _db.Posts.AsQueryable();

            // Apply search filters
            if (!string.IsNullOrWhiteSpace(searchTitle))
                q = q.Where(p => p.Title.Contains(searchTitle));

            if (!string.IsNullOrWhiteSpace(searchContent))
                q = q.Where(p => p.Content.Contains(searchContent));

            q = orderBy switch
            {
                "likes" => q.OrderByDescending(p => _db.Likes.Count(l => l.PostId == p.Id)),
                _ => q.OrderByDescending(p => p.CreatedAt)
            };

            var total = await q.CountAsync();
            Response.Headers.Add("X-Total-Count", total.ToString());
            Response.Headers.Add("Access-Control-Expose-Headers", "X-Total-Count");

            var postRows = await q.Skip((page - 1) * pageSize)
                                  .Take(pageSize)
                                  .Select(p => new
                                  {
                                      p.Id,
                                      p.UserId,
                                      p.UserUsername,
                                      p.Title,
                                      p.Content,
                                      p.CreatedAt,
                                      Likes = _db.Likes.Count(l => l.PostId == p.Id),
                                      IsLikedByMe = _db.Likes.Any(l => l.PostId == p.Id && l.UserId == me)
                                  })
                                  .ToListAsync();

            var postIds = postRows.Select(r => r.Id).ToList();
            var replyRows = await _db.Replies
                                     .Where(r => postIds.Contains(r.PostId!.Value))
                                     .OrderBy(r => r.CreatedAt)
                                     .Select(r => new ReplyDto(
                                         r.Id, r.PostId!.Value, r.UserId, r.UserUsername,
                                         r.Content, r.CreatedAt, r.UserId == me || isAdmin))
                                     .ToListAsync();

            return postRows.Select(p => new PostDto(
                p.Id, p.UserId, p.Title, p.UserUsername, p.Content, p.CreatedAt,
                p.Likes, p.IsLikedByMe,
                replyRows.Where(r => r.PostId == p.Id).ToList(),
                p.UserId == me || isAdmin));
        }


        [Authorize]
        [HttpPost]
        public IActionResult CreatePost(CreatePostRequest req)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId)) return Unauthorized();
            var user = _db.Users.Find(userId)!;
            var post = new Post
            {
                Title = req.Title ?? "",
                Content = req.Content ?? "",
                UserId = userId,
                UserUsername = user.Username,
                CreatedAt = DateTime.UtcNow
            };
            _db.Posts.Add(post);
            _db.SaveChanges();
            return Ok();
        }

        [Authorize]
        [HttpPost("{id}/like")]
        public IActionResult LikePost(int id)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId)) return Unauthorized();

            if (_db.Likes.Any(l => l.PostId == id && l.UserId == userId)) return BadRequest("Already liked");

            var like = new Like { PostId = id, UserId = userId };
            _db.Likes.Add(like);
            _db.SaveChanges();
            return Ok();
        }

        [Authorize]
        [HttpPost("{id}/reply")]
        public IActionResult Reply(int id, CreateReplyRequest req)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = _db.Users.Find(userId)!;                 // get the name once

            var reply = new Reply
            {
                PostId = id,
                UserId = userId,
                UserUsername = user.Username,    // NEW: fill the column
                Content = req.Content ?? "",
                CreatedAt = DateTime.UtcNow
            };
            _db.Replies.Add(reply);
            _db.SaveChanges();
            return Ok();
        }
        // FeedController.cs
        [Authorize]
        [HttpDelete("{id}/like")]
        public IActionResult UnlikePost(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var like = _db.Likes.SingleOrDefault(l => l.PostId == id && l.UserId == userId);
            if (like is null) return NotFound();

            _db.Likes.Remove(like);
            _db.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostRequest dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var post = await _db.Posts.FindAsync(id);
            if (post is null) return NotFound();

            var isAdmin = User.HasClaim("isAdmin", "true");
            if (post.UserId != userId && !isAdmin) return Forbid();

            post.Content = dto.Content;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeletePost(int id)
        {
            var post = _db.Posts.SingleOrDefault(p => p.Id == id);
            if (post is null) return NotFound();

            var me = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var isAdmin = bool.Parse(User.FindFirst("isAdmin")!.Value);

            if (!(isAdmin || post.UserId == me)) return Forbid();

            _db.Posts.Remove(post);
            _db.SaveChanges();
            return NoContent();
        }
    }
}
