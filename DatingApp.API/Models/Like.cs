namespace DatingApp.API.Models
{
    public class Like
    {
        public int LikerId { get; set; } // the liking person
        public int LikeeId { get; set; } // the person we like
        public User Liker { get; set; }
        public User Likee { get; set; }
    }
}