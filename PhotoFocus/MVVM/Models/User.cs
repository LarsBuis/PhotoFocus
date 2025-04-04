﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoFocus.MVVM.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public int Points { get; set; }
        public string ProfilePictureUrl { get; set; }

        public bool IsAdmin { get; set; } = false;
    }
}
