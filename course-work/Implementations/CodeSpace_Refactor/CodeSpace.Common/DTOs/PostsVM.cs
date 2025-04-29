using Common.Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeSpace.Models.FeedInteractions
{
    public class PostsVM
    {
        public List<Post> Posts { get; set; }
        public Dictionary<Post, int> LikesPerPost { get; set; }

        public PostsVM() { 
            LikesPerPost = new Dictionary<Post, int>();
        }
    }
}
