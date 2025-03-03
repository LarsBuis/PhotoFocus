using PhotoFocus.MVVM.Models;
using PhotoFocus.Services;
using System.Collections.ObjectModel;

namespace PhotoFocus.MVVM.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        // The categories displayed in the Picker, including "All Photos"
        private ObservableCollection<Category> _filterCategories;
        public ObservableCollection<Category> FilterCategories
        {
            get => _filterCategories;
            set => SetProperty(ref _filterCategories, value);
        }

        // The category chosen in the Picker
        private Category _selectedFilterCategory;
        public Category SelectedFilterCategory
        {
            get => _selectedFilterCategory;
            set
            {
                if (SetProperty(ref _selectedFilterCategory, value))
                {
                    // Whenever the user picks a different category, we reload photos
                    LoadFilteredPhotos();
                }
            }
        }

        // Display title for current filter
        private string _filterTitle;
        public string FilterTitle
        {
            get => _filterTitle;
            set => SetProperty(ref _filterTitle, value);
        }

        // The main collection of photos to display
        private ObservableCollection<PhotoDisplayItem> _filteredPhotos;
        public ObservableCollection<PhotoDisplayItem> FilteredPhotos
        {
            get => _filteredPhotos;
            set => SetProperty(ref _filteredPhotos, value);
        }

        // Keep all photos in memory so we can quickly filter
        private List<Photo> _allPhotos;

        // Special "All Photos" category
        private Category _allCategory = new Category { Id = -1, Name = "All Photos" };

        public HomeViewModel()
        {
            // Initialize
            _allPhotos = new List<Photo>();
            FilteredPhotos = new ObservableCollection<PhotoDisplayItem>();
            FilterCategories = new ObservableCollection<Category>();

            LoadData();
        }

        private async void LoadData()
        {
            // Load categories from DB
            var cats = await DatabaseService.Database.Table<Category>().ToListAsync();

            // Insert our "All Photos" pseudo-category at the front
            FilterCategories.Add(_allCategory);
            foreach (var cat in cats)
            {
                FilterCategories.Add(cat);
            }

            // Load all photos from DB
            _allPhotos = await DatabaseService.Database.Table<Photo>().ToListAsync();

            // Default to "All Photos"
            SelectedFilterCategory = _allCategory;
        }

        private async void LoadFilteredPhotos()
        {
            if (SelectedFilterCategory == null || SelectedFilterCategory.Id == -1)
            {
                // Show all photos
                FilterTitle = "All Photos";
                await SetPhotosAsync(_allPhotos);
            }
            else
            {
                FilterTitle = SelectedFilterCategory.Name;

                var filtered = _allPhotos
                    .Where(p => p.CategoryId == SelectedFilterCategory.Id)
                    .ToList();

                await SetPhotosAsync(filtered);
            }
        }

        private async Task SetPhotosAsync(List<Photo> photos)
        {
            // Clear out the old results
            FilteredPhotos.Clear();

            // For each photo, find the associated user + category
            foreach (var p in photos)
            {
                var user = await DatabaseService.Database.Table<User>()
                    .Where(u => u.Id == p.UserId)
                    .FirstOrDefaultAsync();

                var category = FilterCategories.FirstOrDefault(c => c.Id == p.CategoryId);

                FilteredPhotos.Add(new PhotoDisplayItem
                {
                    Photo = p,
                    User = user,
                    Category = category
                });
            }
        }
    }

    public class PhotoDisplayItem
    {
        public Photo Photo { get; set; }
        public User User { get; set; }
        public Category Category { get; set; }
    }
}
