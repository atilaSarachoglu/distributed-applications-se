using System.ComponentModel.DataAnnotations;

namespace CodeSpace.Models.FeedInteractions
{
    public class EditPostContentVM
    {
        [Required(ErrorMessage = "You cannot have your edited post be empty.")]
        public string Content { get; set; }
    }
}
