using PhotoFocus.MVVM.Views;
using PhotoFocus.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoFocus.MVVM.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private string _username;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public ICommand RegisterCommand { get; }
        public ICommand CancelCommand { get; }

        public RegisterViewModel()
        {
            RegisterCommand = new Command(async () => await OnRegister());
            CancelCommand = new Command(async () => await OnCancel());
        }

        private async Task OnRegister()
        {
            bool registered = await DatabaseService.RegisterUser(Username, Password);
            if (registered)
            {
                Message = "Registered successfully!";
                await Application.Current.MainPage.Navigation.PushAsync(new LoginPage());
            }
            else
            {
                Message = "Username already exists.";
            }
        }

        private async Task OnCancel()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new LoginPage());
        }
    }
}
