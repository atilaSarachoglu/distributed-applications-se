namespace CodeSpace.Common.ApiDtos
{
    public record LoginRequest(string Username, string Password);
    public record LoginResponse(string Token, string Username, bool IsAdmin, int UserId);
    public record RegisterRequest(string Username, string Password, string Email);
}
