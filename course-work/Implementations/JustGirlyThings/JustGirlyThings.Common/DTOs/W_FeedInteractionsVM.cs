namespace CodeSpace.Models.FeedInteractions
{
    public class W_FeedInteractionsVM
    {
        public UserVM UserVM { get; set; }
        public CreateReplyVM CreateReplyVM { get; set; }
        public CreatePostVM CreatePostVM { get; set; }
        public PostsVM PostsVM { get; set; }
        public RepliesVM RepliesVM { get; set; }
        public EditPostContentVM EditPostContentVM { get; set; }
        public EditPostTitleVM EditPostTitleVM { get; set; }
        public EditReplyContentVM EditReplyContentVM { get; set; }

        public W_FeedInteractionsVM() {
            UserVM = new UserVM();
            CreateReplyVM = new CreateReplyVM();
            CreatePostVM = new CreatePostVM();
            PostsVM = new PostsVM();
            RepliesVM = new RepliesVM();
            EditPostContentVM = new EditPostContentVM();
            EditPostTitleVM = new EditPostTitleVM();
            EditReplyContentVM = new EditReplyContentVM();
        }
    }
}
