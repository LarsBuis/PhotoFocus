using PhotoFocus.MVVM.ViewModels;

namespace PhotoFocus.MVVM.Views
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();

            BindingContext = new ProfileViewModel();
        }
    }
}
