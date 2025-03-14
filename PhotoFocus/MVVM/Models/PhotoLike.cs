using SQLite;

namespace PhotoFocus.MVVM.Models
{
    public class PhotoLike
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int PhotoId { get; set; }

        public int UserId { get; set; }
        public DateTime LikedAt { get; set; }
    }
}
