using System.ComponentModel.DataAnnotations;

namespace CodeSpace.Common.Dtos.User;

public class UpdateUserRequest
{
    [Required, MaxLength(60)]
    public string Username { get; init; } = null!;

    [EmailAddress, MaxLength(100)]
    public string? Email { get; init; }

    [MaxLength(100)]
    public string? Password { get; init; }

    public bool IsAdmin { get; init; }
}
