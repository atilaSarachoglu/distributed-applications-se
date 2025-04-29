using System.ComponentModel.DataAnnotations;

namespace CodeSpace.Models.FeedInteractions
{
    public class CreateReplyVM
    {
        [Required(ErrorMessage = "Your reply cannot be empty.")]
        public string Content { get; set; }
    }
}
