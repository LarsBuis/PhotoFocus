using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoFocus.MVVM.Models;
using SQLite;

namespace PhotoFocus.Services
{
    public class DatabaseService
    {
        private static SQLiteAsyncConnection _database;

        public static SQLiteAsyncConnection Database
        {
            get
            {
                if (_database == null)
                {
                    // Path to local database
                    var dbPath = Path.Combine(FileSystem.AppDataDirectory, "PhotoFocusDB.db3");
                    _database = new SQLiteAsyncConnection(dbPath);
                }
                return _database;
            }
        }

        public static async Task InitializeAsync()
        {
            await Database.CreateTableAsync<User>();
        }

        public static async Task<bool> RegisterUser(string username, string password)
        {
            // Check if the username already exists
            var existingUser = await Database.Table<User>()
                                             .Where(u => u.Username == username)
                                             .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                // Username already taken
                return false;
            }

            var newUser = new User
            {
                Username = username,
                Password = password,
                Points = 5
            };

            int result = await Database.InsertAsync(newUser);
            return result > 0;
        }


        public static async Task<bool> LoginUser(string username, string password)
        {
            var user = await Database.Table<User>()
                .Where(u => u.Username == username && u.Password == password)
                .FirstOrDefaultAsync();

            return user != null;
        }
    }
}
