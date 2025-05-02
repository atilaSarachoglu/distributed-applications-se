using System.ComponentModel.DataAnnotations;

namespace CodeSpace.Common.Dtos.User;

public class UpdateUserRequest
{
    [Required, MaxLength(60)]
    public string Username { get; init; } = null!;

    [EmailAddress, MaxLength(100)]
    public string? Email { get; init; }

    /// <summary>
    /// Plain-text password for now. If null or empty → keep old password.
    /// </summary>
    [MaxLength(100)]
    public string? Password { get; init; }

    public bool IsAdmin { get; init; }
}
