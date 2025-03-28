﻿using PhotoFocus.MVVM.Models;
using PhotoFocus.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Media;

namespace PhotoFocus.MVVM.ViewModels
{
    public class UploadPhotoViewModel : BaseViewModel
    {
        private static readonly HttpClient _httpClient = new HttpClient();

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
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    IsGenerateImageVisible = (_selectedCategory != null);
                }
            }
        }

        private bool _isGenerateImageVisible;
        public bool IsGenerateImageVisible
        {
            get => _isGenerateImageVisible;
            set => SetProperty(ref _isGenerateImageVisible, value);
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

        // We still hold the selected assignment (set externally)
        private Assignment _selectedAssignment;
        public Assignment SelectedAssignment
        {
            get => _selectedAssignment;
            set => SetProperty(ref _selectedAssignment, value);
        }

        // New properties for modifying the API query:
        // These properties are mutually exclusive.
        private bool _useCategoryOnly;
        public bool UseCategoryOnly
        {
            get => _useCategoryOnly;
            set
            {
                if (SetProperty(ref _useCategoryOnly, value) && value)
                {
                    UseBoth = false;
                    UseAssignmentOnly = false;
                }
            }
        }

        private bool _useAssignmentOnly;
        public bool UseAssignmentOnly
        {
            get => _useAssignmentOnly;
            set
            {
                if (SetProperty(ref _useAssignmentOnly, value) && value)
                {
                    UseCategoryOnly = false;
                    UseBoth = false;
                }
            }
        }

        private bool _useBoth;
        public bool UseBoth
        {
            get => _useBoth;
            set
            {
                if (SetProperty(ref _useBoth, value) && value)
                {
                    UseCategoryOnly = false;
                    UseAssignmentOnly = false;
                }
            }
        }

        // Commands for picking, taking, uploading photos, etc.
        public ICommand PickPhotoCommand { get; }
        public ICommand TakePhotoCommand { get; }
        public ICommand UploadCommand { get; }
        public ICommand GeneratePexelsImageCommand { get; }

        // For demo purposes, a static user id
        private int _currentUserId = 1;

        public UploadPhotoViewModel()
        {
            // Set default option; you could default to any
            UseCategoryOnly = true;

            PickPhotoCommand = new Command(async () => await PickPhotoAsync());
            TakePhotoCommand = new Command(async () => await TakePhotoAsync());
            UploadCommand = new Command(async () => await UploadAsync());
            GeneratePexelsImageCommand = new Command(async () => await GeneratePexelsImageAsync());

            LoadCategories();
        }

        private async void LoadCategories()
        {
            var cats = await DatabaseService.Database.Table<Category>().ToListAsync();
            Categories = new ObservableCollection<Category>(cats);
            if (Categories.Count > 0)
                SelectedCategory = Categories[0];
        }

        private async Task GeneratePexelsImageAsync()
        {
            try
            {
                if (SelectedCategory == null)
                {
                    Message = "Please select a category first.";
                    return;
                }

                string queryTerm;
                if (UseCategoryOnly)
                {
                    queryTerm = SelectedCategory.Name;
                }
                else if (UseAssignmentOnly)
                {
                    queryTerm = SelectedAssignment != null ? SelectedAssignment.Title : "";
                }
                else if (UseBoth)
                {
                    queryTerm = SelectedCategory.Name + " " + (SelectedAssignment != null ? SelectedAssignment.Title : "");
                }
                else
                {
                    queryTerm = SelectedCategory.Name;
                }

                queryTerm = Uri.EscapeDataString(queryTerm);

                var random = new Random();
                int randomPage = random.Next(1, 25);
                string requestUrl = $"https://api.pexels.com/v1/search?query={queryTerm}&per_page=1&page={randomPage}";

                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Add("Authorization", "d4u7gyfx95QgvrUXJPAlqwZWJ44XSnULe9StNrDF8yAU1HrcphZNGrWK");

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    Message = $"Pexels request failed: {response.StatusCode}";
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("Pexels JSON response: " + json);

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (!root.TryGetProperty("photos", out JsonElement photos))
                {
                    Message = "Unexpected JSON format: no 'photos' property.";
                    return;
                }

                if (photos.GetArrayLength() == 0)
                {
                    Message = $"No images found on Pexels for query '{queryTerm}'.";
                    return;
                }

                var firstPhoto = photos[0];
                if (!firstPhoto.TryGetProperty("src", out JsonElement src))
                {
                    Message = "Unexpected JSON format: no 'src' property in photo.";
                    return;
                }

                string originalUrl = src.GetProperty("original").GetString();
                var localPath = await DownloadImageToLocalPath(originalUrl);
                SelectedImagePath = localPath;

                Message = $"{queryTerm} image fetched from Pexels!";
            }
            catch (Exception ex)
            {
                Message = $"Error generating image: {ex.Message}";
            }
        }

        private async Task<string> DownloadImageToLocalPath(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var fileName = $"{Guid.NewGuid()}.jpg";
            var localPath = Path.Combine(FileSystem.AppDataDirectory, fileName);

            await using var ms = await response.Content.ReadAsStreamAsync();
            await using var fs = File.OpenWrite(localPath);
            await ms.CopyToAsync(fs);

            return localPath;
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
                    var localPath = await CopyFileToLocalPath(result);
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
                    var localPath = await CopyFileToLocalPath(photo);
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
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var localPath = Path.Combine(FileSystem.AppDataDirectory, fileName);

            using var stream = await file.OpenReadAsync();
            using var newStream = File.OpenWrite(localPath);
            await stream.CopyToAsync(newStream);

            return localPath;
        }

        private async Task UploadAsync()
        {
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

            bool success = await DatabaseService.AddPhoto(_currentUserId, SelectedCategory.Id, SelectedImagePath, SelectedAssignment?.Id);
            if (success)
            {
                Message = "Photo uploaded successfully!";
                SelectedImagePath = null;
            }
            else
            {
                Message = "Failed to upload photo.";
            }
        }
    }
}
