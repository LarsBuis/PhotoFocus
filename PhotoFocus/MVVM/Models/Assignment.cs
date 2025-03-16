using SQLite;
using System;

namespace PhotoFocus.MVVM.Models
{
    public class Assignment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public int DurationDays { get; set; }

        // Compute the end date based on StartDate and DurationDays
        [Ignore]
        public DateTime EndDate => StartDate.AddDays(DurationDays);

        // Computed property that calculates the time remaining
        [Ignore]
        public string TimeRemaining
        {
            get
            {
                var remaining = EndDate - DateTime.Now;
                if (remaining.TotalSeconds <= 0)
                    return "Expired";
                return $"{remaining.Days} days remaining";
            }
        }
    }
}
