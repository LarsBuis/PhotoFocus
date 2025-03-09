using PhotoFocus.MVVM.ViewModels;
using PhotoFocus.Services;

namespace PhotoFocus.MVVM.Views;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();

        BindingContext = new HomeViewModel(new NavigationService(this.Navigation));
    }
}