using System.ComponentModel.DataAnnotations;

namespace CodeSpace.Models.FeedInteractions
{
    public class CreateReplyVM
    {
        [Required(ErrorMessage = "Your reply cannot be empty.")]
        [StringLength(1000)]
        public string Content { get; set; }
    }
}
