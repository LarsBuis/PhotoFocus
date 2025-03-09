using PhotoFocus.MVVM.Views;

namespace PhotoFocus.Services
{
    public class NavigationService : INavigationService
    {
        private readonly INavigation _navigation;

        // Pass in an INavigation from the Page's Navigation property
        public NavigationService(INavigation navigation)
        {
            _navigation = navigation;
        }

        public Task NavigateToUploadPhotoAsync()
        {
            // Push the UploadPhotoPage on the current navigation stack
            return _navigation.PushAsync(new UploadPhotoPage());
        }
    }
}
