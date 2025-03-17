namespace PhotoFocus.MVVM.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
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