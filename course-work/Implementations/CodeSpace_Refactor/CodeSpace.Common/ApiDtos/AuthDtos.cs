using System.ComponentModel.DataAnnotations;

namespace CodeSpace.Common.ApiDtos
{
    public record LoginRequest(
        [Required, MaxLength(50)] string Username,
        [Required, MaxLength(64)] string Password);

    public record LoginResponse(string Token, string Username, bool IsAdmin, int UserId);

    public record RegisterRequest(
        [Required, MaxLength(50)] string Username,
        [Required, MaxLength(64)] string Password,
        [Required, EmailAddress, MaxLength(255)] string Email);
}