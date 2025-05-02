namespace CodeSpace.Common.Dtos.User;

/// <summary>Light-weight projection returned by GET /api/Users</summary>
public record UserDto(int Id, string Username, bool IsAdmin);
