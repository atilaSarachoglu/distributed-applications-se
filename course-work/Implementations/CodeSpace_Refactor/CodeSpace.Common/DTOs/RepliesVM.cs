using Common.Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeSpace.Models.FeedInteractions
{
    public class RepliesVM
    {
        public List<Reply> Replies { get; set; }

        public Dictionary<Post, List<Reply>> RepliesPerPost { get; set; }

        public RepliesVM()
        {
            RepliesPerPost = new Dictionary<Post, List<Reply>>();
        }
    }
}
