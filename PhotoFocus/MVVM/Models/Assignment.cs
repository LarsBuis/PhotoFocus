using SQLite;
using System;

namespace PhotoFocus.MVVM.Models
{
    public class Assignment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // A short title for the assignment
        public string Title { get; set; }

        // Detailed description if needed
        public string Description { get; set; }

        // When the assignment is created or starts
        public DateTime StartDate { get; set; }

        // Duration in days (for example, 7 or 14)
        public int DurationDays { get; set; }

        // Optional: calculated end date (not stored in DB)
        [Ignore]
        public DateTime EndDate => StartDate.AddDays(DurationDays);
    }
}
