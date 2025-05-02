using System;
using System.Collections.Generic;

namespace CodeSpace.Common.ApiDtos
{
    public record PostDto(int Id, int UserId, string Title, string UserUsername,
        string Content, DateTime CreatedAt, int Likes, bool IsLikedByMe, List<ReplyDto> Replies, bool CanEdit);
    public record CreatePostRequest(string Title, string Content);
    public record ReplyDto(int Id, int PostId, int UserId, string UserUsername, string Content, DateTime CreatedAt, bool CanEdit);
    public record CreateReplyRequest(int PostId, string Content);
}
