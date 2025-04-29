using Common.Entities;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Repos
{
    public class LikesRepo : BaseRepo<Like>
    {
        private DbContext DbContext { get; set; }
        private DbSet<Like> Likes { get; set; }

        public LikesRepo()
        {
            DbContext = new AppDbContext();
            Likes = DbContext.Set<Like>();
        }

        public int GetLikesPerPost(int postId)
        {
            return Likes.Where(x=>x.PostId == postId).Count();
        }
    }
}
