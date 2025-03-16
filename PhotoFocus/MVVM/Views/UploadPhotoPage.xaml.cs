using Microsoft.Maui.Graphics;

namespace PhotoFocus.MVVM.Views
{
    public partial class UploadPhotoPage : ContentPage
    {
        public UploadPhotoPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // If this page is wrapped in a NavigationPage, set the bar colors
            if (Application.Current.MainPage is NavigationPage navPage)
            {
                navPage.BarBackgroundColor = Color.FromArgb("#222431");
                navPage.BarTextColor = Colors.White;
            }
        }
    }
}
