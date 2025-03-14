using Microsoft.Maui.Storage;
using PhotoFocus.MVVM.Views;
using PhotoFocus.Services;

namespace PhotoFocus
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var userId = SecureStorage.Default.GetAsync("userId")
                                             .GetAwaiter()
                                             .GetResult();

            Task.Run(async () => await DatabaseService.InitializeAsync());


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
