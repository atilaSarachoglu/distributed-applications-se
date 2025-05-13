using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeSpace.Common.ApiDtos
{
    public record PostDto(
        int Id,
        int UserId,
        string Title,
        string UserUsername,
        string Content,
        DateTime CreatedAt,
        int Likes,
        bool IsLikedByMe,
        List<ReplyDto> Replies,
        bool CanEdit);

    public record CreatePostRequest(
        [Required, StringLength(15)] string Title,
        [Required, MaxLength(1000)] string Content);

    public record ReplyDto(
        int Id,
        int PostId,
        int UserId,
        string UserUsername,
        string Content,
        DateTime CreatedAt,
        bool CanEdit);

    public record CreateReplyRequest(
        int PostId,
        [Required, MaxLength(1000)] string Content);
}
