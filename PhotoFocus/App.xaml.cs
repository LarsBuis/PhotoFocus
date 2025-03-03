using Microsoft.Maui.Storage;
using PhotoFocus.MVVM.Views;

namespace PhotoFocus
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Synchronous read from SecureStorage
            var userId = SecureStorage.Default.GetAsync("userId")
                                             .GetAwaiter()
                                             .GetResult();

            if (!string.IsNullOrEmpty(userId))
            {
                MainPage = new NavigationPage(new MainPage());
            }
            else
            {
                MainPage = new NavigationPage(new LoginPage());
            }
        }
    }
}
