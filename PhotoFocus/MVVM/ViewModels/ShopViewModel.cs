using PhotoFocus.MVVM.Models;
using PhotoFocus.Services;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Storage;

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

        public ICommand BuyMembershipCommand { get; }

        public ShopViewModel()
        {
            BuyMembershipCommand = new Command(async () => await BuyMembershipAsync());
            InitializeUserIdAsync();
        }

        private async void InitializeUserIdAsync()
        {
            var storedUserId = await SecureStorage.GetAsync("userId");
            if (!string.IsNullOrEmpty(storedUserId) && int.TryParse(storedUserId, out int id))
            {
                _currentUserId = id;
            }
            await CheckMembershipStatusAsync();
        }

        private async Task BuyMembershipAsync()
        {
            bool active = await DatabaseService.IsMembershipActiveAsync(_currentUserId);
            if (active)
            {
                IsMember = true;
                return;
            }

            bool result = await DatabaseService.AddMembershipAsync(_currentUserId);
            if (result)
            {
                IsMember = true;
            }
            else
            {
                // have to add error message
            }
        }

        private async Task CheckMembershipStatusAsync()
        {
            IsMember = await DatabaseService.IsMembershipActiveAsync(_currentUserId);
        }
    }
}
