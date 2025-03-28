﻿using SQLite;

namespace PhotoFocus.MVVM.Models
{
    public class Membership
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
