using Common.Entities;

using System.Collections.Generic;

namespace CodeSpace.Models.FeedInteractions
{
    public class UserVM
    {
        public string Username { get; set;}
        public bool IsAdmin { get; set;}
        public List<Like> LikedPosts { get; set;}

        public UserVM() { 
            LikedPosts = new List<Like>();
        }
    }
}
