using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities
{
    public class Reply : BaseEntity
    {
        public int UserId {  get; set; }
        public int? PostId { get; set; }
        public string Content { get; set; }
        public string UserUsername { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual User User { get; set; }
        public virtual Post Post { get; set; }
    }
}
