using Microsoft.Maui.Storage;
using PhotoFocus.MVVM.Models;
using PhotoFocus.MVVM.Views;
using PhotoFocus.Services;
using System.Threading.Tasks;
using System.Windows.Input;
using SQLite;

namespace PhotoFocus.MVVM.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public ICommand ChangeProfilePictureCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand SettingsCommand { get; } // New command for settings

        public ProfileViewModel()
        {
            // For demonstration, just load the first user
            // In a real app, you'd identify the actual logged-in user
            LoadUser();

            // Existing command to change profile picture
            ChangeProfilePictureCommand = new Command(async () => await OnChangeProfilePicture());

            // Logout command
            LogoutCommand = new Command(OnLogout);

            // New: Initialize the SettingsCommand
            SettingsCommand = new Command(async () => await OnSettings());
        }

        private async void LoadUser()
        {
            // Retrieve the user id from SecureStorage using the key "userId"
            var storedUserId = await SecureStorage.GetAsync("userId");
            if (!string.IsNullOrEmpty(storedUserId) && int.TryParse(storedUserId, out int userId))
            {
                // Load the user from the database by userId
                CurrentUser = await DatabaseService.Database.Table<User>()
                    .Where(u => u.Id == userId)
                    .FirstOrDefaultAsync();
            }
        }

        // Logout: Remove stored userId and navigate to LoginPage
        private void OnLogout()
        {
            SecureStorage.Default.Remove("userId");
            Application.Current.MainPage = new LoginPage();
        }

        // New: Navigate to SettingsPage
        private async Task OnSettings()
        {
            // Ensure you have a SettingsPage in your project
            await Application.Current.MainPage.Navigation.PushAsync(new SettingsPage());
        }

        private async Task OnChangeProfilePicture()
        {
            // Prompts user to pick from gallery or take photo
            var action = await App.Current.MainPage.DisplayActionSheet(
                "Change profile picture",
                "Cancel", null,
                "Pick from gallery",
                "Take a new photo");

            if (action == "Pick from gallery")
            {
                await PickPhotoFromGallery();
            }
            else if (action == "Take a new photo")
            {
                await TakePhotoWithCamera();
            }
        }

        private async Task PickPhotoFromGallery()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images,
                    PickerTitle = "Pick a profile picture"
                });

                if (result != null)
                {
                    var newFilePath = await CopyFileToLocalPath(result);
                    CurrentUser.ProfilePictureUrl = newFilePath;
                    await UpdateUserInDatabase();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private async Task TakePhotoWithCamera()
        {
            try
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo != null)
                {
                    var newFilePath = await CopyFileToLocalPath(photo);
                    CurrentUser.ProfilePictureUrl = newFilePath;
                    await UpdateUserInDatabase();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private async Task<string> CopyFileToLocalPath(FileResult file)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var localPath = System.IO.Path.Combine(FileSystem.AppDataDirectory, fileName);

            using (var stream = await file.OpenReadAsync())
            using (var newStream = System.IO.File.OpenWrite(localPath))
            {
                await stream.CopyToAsync(newStream);
            }

            return localPath;
        }

        private async Task UpdateUserInDatabase()
        {
            await DatabaseService.Database.UpdateAsync(CurrentUser);
        }
    }
}
