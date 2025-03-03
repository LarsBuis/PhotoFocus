using SQLite;

namespace PhotoFocus.MVVM.Models
{
    public class Photo
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string FilePath { get; set; }

        public int UserId { get; set; }

        public int CategoryId { get; set; }

        public DateTime UploadedAt { get; set; }
    }
}
