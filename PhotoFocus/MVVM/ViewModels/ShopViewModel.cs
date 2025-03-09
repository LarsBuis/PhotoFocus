using PhotoFocus.MVVM.Models;
using PhotoFocus.Services;
using System.Windows.Input;

namespace PhotoFocus.MVVM.ViewModels
{
    public class ShopViewModel : BaseViewModel
    {
        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        // Command that executes when the user taps "Buy"
        public ICommand BuyCommand { get; }

        public ShopViewModel()
        {
            LoadUser();

            // Initialize the BuyCommand
            BuyCommand = new Command(async () => await OnBuy());
        }

        private async void LoadUser()
        {
            // For demo, load the first user from DB
            // In a real app, you'd load the *currently logged-in user*
            var users = await DatabaseService.Database.Table<User>().ToListAsync();
            CurrentUser = users.FirstOrDefault();
        }

        private async Task OnBuy()
        {
            if (CurrentUser == null)
            {
                Message = "No user loaded.";
                return;
            }

            // Increase the user's points by 1 (cost = 1 euro)
            CurrentUser.Points += 1;

            // Update the database
            await DatabaseService.Database.UpdateAsync(CurrentUser);

            // Provide feedback to the UI
            Message = $"You bought 1 point! You now have {CurrentUser.Points} points.";
        }
    }
}
