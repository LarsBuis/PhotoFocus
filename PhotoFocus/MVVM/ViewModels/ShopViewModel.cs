using PhotoFocus.MVVM.Models;
using PhotoFocus.Services;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Storage;
using SQLite;
using System;

namespace PhotoFocus.MVVM.ViewModels
{
    public class ShopViewModel : BaseViewModel
    {
        private bool _isMember;
        public bool IsMember
        {
            get => _isMember;
            set => SetProperty(ref _isMember, value);
        }

        private int _currentUserId;

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        // New properties for display
        private int _currentPoints;
        public int CurrentPoints
        {
            get => _currentPoints;
            set => SetProperty(ref _currentPoints, value);
        }

        private string _membershipDuration;
        public string MembershipDuration
        {
            get => _membershipDuration;
            set => SetProperty(ref _membershipDuration, value);
        }

        public ICommand BuyMembershipCommand { get; }
        public ICommand BuyOnePointCommand { get; }
        public ICommand BuyFivePointsCommand { get; }
        public ICommand BuyTenPointsCommand { get; }

        public ShopViewModel()
        {
            BuyMembershipCommand = new Command(async () => await BuyMembershipAsync());
            BuyOnePointCommand = new Command(async () => await BuyOnePointAsync());
            BuyFivePointsCommand = new Command(async () => await BuyFivePointsAsync());
            BuyTenPointsCommand = new Command(async () => await BuyTenPointsAsync());

            InitializeUserIdAsync();
        }

        private async void InitializeUserIdAsync()
        {
            var storedUserId = await SecureStorage.GetAsync("userId");
            if (!string.IsNullOrEmpty(storedUserId) && int.TryParse(storedUserId, out int id))
            {
                _currentUserId = id;
            }
            await UpdateUserInfoAsync();
            await CheckMembershipStatusAsync();
        }

        // Retrieves and updates the current user's points and membership details.
        private async Task UpdateUserInfoAsync()
        {
            // Update current points
            var user = await DatabaseService.Database.Table<User>()
                            .Where(u => u.Id == _currentUserId)
                            .FirstOrDefaultAsync();
            if (user != null)
            {
                CurrentPoints = user.Points;
            }

            // Retrieve membership record and compute duration
            var membership = await DatabaseService.Database.Table<Membership>()
                                .Where(m => m.UserId == _currentUserId)
                                .FirstOrDefaultAsync();
            if (membership != null)
            {
                if (membership.EndDate > DateTime.UtcNow)
                {
                    var daysLeft = (membership.EndDate - DateTime.UtcNow).Days;
                    MembershipDuration = $"{daysLeft} days left";
                }
                else
                {
                    MembershipDuration = "Expired";
                }
            }
            else
            {
                MembershipDuration = "Not a member";
            }
        }

        private async Task BuyMembershipAsync()
        {
            bool active = await DatabaseService.IsMembershipActiveAsync(_currentUserId);
            if (active)
            {
                IsMember = true;
                Message = "Membership is already active.";
                await UpdateUserInfoAsync();
                return;
            }

            bool result = await DatabaseService.AddMembershipAsync(_currentUserId);
            if (result)
            {
                IsMember = true;
                Message = "Membership purchased successfully!";
            }
            else
            {
                Message = "Failed to purchase membership.";
            }
            await UpdateUserInfoAsync();
        }

        private async Task CheckMembershipStatusAsync()
        {
            IsMember = await DatabaseService.IsMembershipActiveAsync(_currentUserId);
        }

        private async Task BuyOnePointAsync()
        {
            var user = await DatabaseService.Database.Table<User>()
                            .Where(u => u.Id == _currentUserId)
                            .FirstOrDefaultAsync();
            if (user != null)
            {
                user.Points += 1;
                await DatabaseService.Database.UpdateAsync(user);
                Message = "1 point purchased!";
                await UpdateUserInfoAsync();
            }
        }

        private async Task BuyFivePointsAsync()
        {
            var user = await DatabaseService.Database.Table<User>()
                            .Where(u => u.Id == _currentUserId)
                            .FirstOrDefaultAsync();
            if (user != null)
            {
                user.Points += 5;
                await DatabaseService.Database.UpdateAsync(user);
                Message = "5 points purchased!";
                await UpdateUserInfoAsync();
            }
        }

        private async Task BuyTenPointsAsync()
        {
            var user = await DatabaseService.Database.Table<User>()
                            .Where(u => u.Id == _currentUserId)
                            .FirstOrDefaultAsync();
            if (user != null)
            {
                // 10 points with 30% discount (for demo: simply add 10 points)
                user.Points += 10;
                await DatabaseService.Database.UpdateAsync(user);
                Message = "10 points purchased with discount!";
                await UpdateUserInfoAsync();
            }
        }
    }
}
