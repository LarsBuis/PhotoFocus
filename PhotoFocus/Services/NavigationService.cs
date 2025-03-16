using PhotoFocus.MVVM.Views;

namespace PhotoFocus.Services
{
    public class NavigationService : INavigationService
    {
        private readonly INavigation _navigation;

        public NavigationService(INavigation navigation)
        {
            _navigation = navigation;
        }

        public Task NavigateToUploadPhotoAsync()
        {
            return _navigation.PushAsync(new UploadPhotoPage());
        }
    }
}
