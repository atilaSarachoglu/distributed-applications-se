using Common.Entities;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Repos
{
    public class RepliesRepo : BaseRepo<Reply>
    {
        private DbContext DbContext { get; set; }
        private DbSet<Reply> Replies { get; set; }

        public RepliesRepo()
        {
            DbContext = new AppDbContext();
            Replies = DbContext.Set<Reply>();
        }

        public List<Reply> GetRepliesPerPost(int postId)
        {
            return Replies.Where(x => x.PostId == postId).ToList();
        }
    }
}
