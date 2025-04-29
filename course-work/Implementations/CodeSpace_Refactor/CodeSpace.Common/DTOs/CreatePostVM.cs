using System.ComponentModel.DataAnnotations;

namespace CodeSpace.Models.FeedInteractions
{
    public class CreatePostVM
    {
        [Required(ErrorMessage = "You cannot have an empty title.")]
        [StringLength(15)]
        public string Title { get; set; }   
        [Required(ErrorMessage = "You cannot have an empty body.")]
        [StringLength(300)]
        public string Content { get; set; }
        public int UserId { get; set; }
        public string UserUsername { get; set; }
    }
}
