using PhotoFocus.MVVM.ViewModels;

namespace PhotoFocus.MVVM.Views
{
    public partial class ShopPage : ContentPage
    {
        public ShopPage()
        {
            InitializeComponent();
            BindingContext = new ShopViewModel();
        }
    }
}
