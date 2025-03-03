﻿using PhotoFocus.MVVM.Models;
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
    public class LoginViewModel : BaseViewModel
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

        public ICommand LoginCommand { get; }
        public ICommand GoToRegisterCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(async () => await OnLogin());
            GoToRegisterCommand = new Command(async () => await OnGoToRegister());
        }

        private async Task OnLogin()
        {
            bool isValid = await DatabaseService.LoginUser(Username, Password);
            if (isValid)
            {
                // 1) Fetch the user record from DB
                var user = await DatabaseService.Database.Table<User>()
                    .Where(u => u.Username == Username)
                    .FirstOrDefaultAsync();

                // 2) Store the user’s ID in SecureStorage so we remember them
                if (user != null)
                {
                    await SecureStorage.Default.SetAsync("userId", user.Id.ToString());
                }

                // 3) Navigate to the main page
                Application.Current.MainPage = new MainPage();
            }
            else
            {
                Message = "Invalid username or password.";
            }
        }


        private async Task OnGoToRegister()
        {
            // Navigate to RegisterPage (requires Shell routes or Navigation)
            await Application.Current.MainPage.Navigation.PushAsync(new RegisterPage());
        }
    }
}
