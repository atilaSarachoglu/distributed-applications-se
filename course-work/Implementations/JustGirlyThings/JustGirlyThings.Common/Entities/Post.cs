using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities
{
    public class Post : BaseEntity
    {
        public int UserId { get; set; }
        public string Title { get; set; }
        public string UserUsername { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual User User { get; set; }
    }
}
