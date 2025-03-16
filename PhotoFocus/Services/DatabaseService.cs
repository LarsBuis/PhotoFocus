using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PhotoFocus.MVVM.Models;
using SQLite;
using Microsoft.Maui.Storage;

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
                    var dbPath = Path.Combine(FileSystem.AppDataDirectory, "PhotoFocusDB3.db3");
                    _database = new SQLiteAsyncConnection(dbPath);
                }
                return _database;
            }
        }

        public static async Task InitializeAsync()
        {
            await Database.CreateTableAsync<User>();
            await Database.CreateTableAsync<Category>();
            await Database.CreateTableAsync<Photo>();
            await Database.CreateTableAsync<PhotoLike>();
            await Database.CreateTableAsync<Membership>();
            await Database.CreateTableAsync<Assignment>();

            await SeedCategoriesAsync();
        }

        private static async Task SeedCategoriesAsync()
        {
            var categories = await Database.Table<Category>().ToListAsync();
            if (!categories.Any())
            {
                await Database.InsertAllAsync(new List<Category>
                {
                    new Category { Name = "Black and White" },
                    new Category { Name = "Night" },
                    new Category { Name = "Underwater" },
                    new Category { Name = "Nature" },
                    new Category { Name = "Portrait" }
                });
            }
        }

        public static async Task<bool> AddPhoto(int userId, int categoryId, string filePath, int? assignmentId = null)
        {
            var photo = new Photo
            {
                UserId = userId,
                CategoryId = categoryId,
                FilePath = filePath,
                UploadedAt = DateTime.Now,
                AssignmentId = assignmentId
            };

            int result = await Database.InsertAsync(photo);
            return result > 0;
        }


        public static async Task<bool> ToggleLikeAsync(int photoId, int userId)
        {
            var existingLike = await Database.Table<PhotoLike>()
                .Where(l => l.PhotoId == photoId && l.UserId == userId)
                .FirstOrDefaultAsync();

            if (existingLike == null)
            {
                var newLike = new PhotoLike
                {
                    PhotoId = photoId,
                    UserId = userId,
                    LikedAt = DateTime.UtcNow
                };
                await Database.InsertAsync(newLike);
                return true;
            }
            else
            {
                await Database.DeleteAsync(existingLike);
                return false;
            }
        }

        public static async Task<int> GetLikeCountAsync(int photoId)
        {
            int likeCount = await Database.Table<PhotoLike>()
                .Where(l => l.PhotoId == photoId)
                .CountAsync();
            return likeCount;
        }

        public static async Task<bool> RegisterUser(string username, string password)
        {
            var existingUser = await Database.Table<User>()
                                             .Where(u => u.Username == username)
                                             .FirstOrDefaultAsync();

            if (existingUser != null)
            {
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

        public static async Task<User> ValidateUserAndGetAsync(string username, string password)
        {
            var user = await Database.Table<User>()
                .Where(u => u.Username == username && u.Password == password)
                .FirstOrDefaultAsync();
            return user;
        }

        public static async Task<bool> AddMembershipAsync(int userId)
        {
            var membership = new Membership
            {
                UserId = userId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30)
            };
            int result = await Database.InsertAsync(membership);
            return result > 0;
        }

        public static async Task<bool> IsMembershipActiveAsync(int userId)
        {
            var membership = await Database.Table<Membership>()
                .Where(m => m.UserId == userId && m.EndDate > DateTime.UtcNow)
                .FirstOrDefaultAsync();
            return membership != null;
        }
    }
}
