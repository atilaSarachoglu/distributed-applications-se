using System.ComponentModel.DataAnnotations;

namespace CodeSpace.Common.Dtos.Feed;
public record UpdateReplyRequest(
    [Required, MaxLength(600)] string Content);
