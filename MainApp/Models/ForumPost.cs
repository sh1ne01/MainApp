namespace MainApp.Models
{
    public class ForumPost
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public string UserLogin { get; set; }
        public string Role { get; set; }
        public string? AvatarPath { get; set; }
    }
}
