using System.ComponentModel.DataAnnotations;

namespace CodeSpace.Models.FeedInteractions
{
    public class EditReplyContentVM
    {
        [Required(ErrorMessage = "You cannot have your edited reply be empty.")]
        public string Content { get; set; }
    }
}
