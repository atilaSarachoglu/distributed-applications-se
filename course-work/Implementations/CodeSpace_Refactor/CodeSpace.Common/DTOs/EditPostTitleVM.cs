using System.ComponentModel.DataAnnotations;

namespace CodeSpace.Models.FeedInteractions
{
    public class EditPostTitleVM
    {
        [Required(ErrorMessage = "You cannot have your edited title be empty.")]
        [StringLength(50)]
        public string Title { get; set; }
    }
}
