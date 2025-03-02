using PhotoFocus.MVVM.Views;
using PhotoFocus.Services;

namespace PhotoFocus
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DatabaseService.InitializeAsync();

            MainPage = new NavigationPage(new LoginPage());
        }
    }
}
