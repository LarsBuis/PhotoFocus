using System.Windows.Input;
using PhotoFocus.MVVM.Models;
using PhotoFocus.MVVM.ViewModels;
using PhotoFocus.Services;

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

        public ProfileViewModel()
        {
            // For demo, let's just load the first user from the DB
            // In a real app, you'd identify which user is logged in
            LoadUser();

            ChangeProfilePictureCommand = new Command(async () => await OnChangeProfilePicture());
        }

        private async void LoadUser()
        {
            var users = await DatabaseService.Database.Table<User>().ToListAsync();

            // For demonstration, just pick the first user
            // In a real app, retrieve the actual logged-in user
            CurrentUser = users.FirstOrDefault();
        }

        private async Task OnChangeProfilePicture()
        {
            // Option A: Let the user pick from gallery or take photo in one step
            // or Option B: Provide two separate buttons or prompts
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
                    // Copy the file to the local app data directory
                    var newFilePath = await CopyFileToLocalPath(result);
                    CurrentUser.ProfilePictureUrl = newFilePath;
                    await UpdateUserInDatabase();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g. user canceled or permissions issue
                Console.WriteLine(ex);
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
                // Handle camera not available, permissions, cancellation, etc.
                Console.WriteLine(ex);
            }
        }

        private async Task<string> CopyFileToLocalPath(FileResult file)
        {
            // Create a unique filename and path in the local folder
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var localPath = Path.Combine(FileSystem.AppDataDirectory, fileName);

            using (var stream = await file.OpenReadAsync())
            using (var newStream = File.OpenWrite(localPath))
            {
                await stream.CopyToAsync(newStream);
            }

            return localPath;
        }

        private async Task UpdateUserInDatabase()
        {
            // Update the user's info in SQLite
            await DatabaseService.Database.UpdateAsync(CurrentUser);
        }
    }
}
