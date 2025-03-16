using PhotoFocus.MVVM.ViewModels;

namespace PhotoFocus.MVVM.Views;

public partial class CreateAssignmentPage : ContentPage
{
	public CreateAssignmentPage()
	{
        InitializeComponent();

        BindingContext = new CreateAssignmentViewModel();
    }
}