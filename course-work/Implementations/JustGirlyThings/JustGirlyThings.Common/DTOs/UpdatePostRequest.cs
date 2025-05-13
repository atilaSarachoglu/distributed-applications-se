using System.ComponentModel.DataAnnotations;

namespace CodeSpace.Common.Dtos.Feed;
public record UpdatePostRequest(
    [Required, MaxLength(1000)] string Content);
