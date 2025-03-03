using PhotoFocus.MVVM.Models;
using PhotoFocus.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PhotoFocus.MVVM.ViewModels
{
    public class UploadPhotoViewModel : BaseViewModel
    {
        private ObservableCollection<Category> _categories;
        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        private string _selectedImagePath;
        public string SelectedImagePath
        {
            get => _selectedImagePath;
            set => SetProperty(ref _selectedImagePath, value);
        }

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public ICommand PickPhotoCommand { get; }
        public ICommand TakePhotoCommand { get; }
        public ICommand UploadCommand { get; }

        // For demo, let's store a "logged-in" user ID here
        private int _currentUserId = 1; // In a real app, you'd get this from your login system

        public UploadPhotoViewModel()
        {
            PickPhotoCommand = new Command(async () => await PickPhotoAsync());
            TakePhotoCommand = new Command(async () => await TakePhotoAsync());
            UploadCommand = new Command(async () => await UploadAsync());

            LoadCategories();
        }

        private async void LoadCategories()
        {
            var cats = await DatabaseService.Database.Table<Category>().ToListAsync();
            Categories = new ObservableCollection<Category>(cats);

            // Optional: auto-select first category
            if (Categories.Count > 0)
                SelectedCategory = Categories[0];
        }

        private async Task PickPhotoAsync()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images,
                    PickerTitle = "Pick a photo"
                });

                if (result != null)
                {
                    // Copy it to local app folder
                    string localPath = await CopyFileToLocalPath(result);
                    SelectedImagePath = localPath;
                }
            }
            catch (Exception ex)
            {
                Message = $"Error picking photo: {ex.Message}";
            }
        }

        private async Task TakePhotoAsync()
        {
            try
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo != null)
                {
                    string localPath = await CopyFileToLocalPath(photo);
                    SelectedImagePath = localPath;
                }
            }
            catch (Exception ex)
            {
                Message = $"Error taking photo: {ex.Message}";
            }
        }

        private async Task<string> CopyFileToLocalPath(FileResult file)
        {
            // Make a unique filename
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var localPath = Path.Combine(FileSystem.AppDataDirectory, fileName);

            using var stream = await file.OpenReadAsync();
            using var newStream = File.OpenWrite(localPath);
            await stream.CopyToAsync(newStream);

            return localPath;
        }

        private async Task UploadAsync()
        {
            // Validate
            if (string.IsNullOrEmpty(SelectedImagePath))
            {
                Message = "Please pick or take a photo first.";
                return;
            }
            if (SelectedCategory == null)
            {
                Message = "Please select a category.";
                return;
            }

            // Add the photo to DB
            bool success = await DatabaseService.AddPhoto(_currentUserId, SelectedCategory.Id, SelectedImagePath);
            if (success)
            {
                Message = "Photo uploaded successfully!";
                // Optionally clear the selection or navigate away
                SelectedImagePath = null;
            }
            else
            {
                Message = "Failed to upload photo.";
            }
        }
    }
}
