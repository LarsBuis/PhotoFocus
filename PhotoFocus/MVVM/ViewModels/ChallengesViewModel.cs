using PhotoFocus.MVVM.Models;
using PhotoFocus.Services;
using System.Collections.ObjectModel;

namespace PhotoFocus.MVVM.ViewModels
{
    public class ChallengesViewModel : BaseViewModel
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
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    // Load photos for the selected category whenever it changes
                    LoadCategoryPhotos();
                }
            }
        }

        private string _categoryTitle;
        public string CategoryTitle
        {
            get => _categoryTitle;
            set => SetProperty(ref _categoryTitle, value);
        }

        private ObservableCollection<PhotoDisplayItem> _categoryPhotos;
        public ObservableCollection<PhotoDisplayItem> CategoryPhotos
        {
            get => _categoryPhotos;
            set => SetProperty(ref _categoryPhotos, value);
        }

        public ChallengesViewModel()
        {
            LoadCategories();
        }

        private async void LoadCategories()
        {
            var cats = await DatabaseService.Database.Table<Category>().ToListAsync();
            Categories = new ObservableCollection<Category>(cats);
        }

        private async void LoadCategoryPhotos()
        {
            if (SelectedCategory == null)
            {
                CategoryPhotos = new ObservableCollection<PhotoDisplayItem>();
                CategoryTitle = "";
                return;
            }

            // Update title
            CategoryTitle = $"Photos for {SelectedCategory.Name}";

            // Get photos for this category
            var photos = await DatabaseService.Database.Table<Photo>()
                .Where(p => p.CategoryId == SelectedCategory.Id)
                .ToListAsync();

            // For each photo, fetch the user
            var items = new List<PhotoDisplayItem>();
            foreach (var p in photos)
            {
                var user = await DatabaseService.Database.Table<User>()
                    .Where(u => u.Id == p.UserId)
                    .FirstOrDefaultAsync();

                items.Add(new PhotoDisplayItem
                {
                    Photo = p,
                    User = user
                });
            }

            CategoryPhotos = new ObservableCollection<PhotoDisplayItem>(items);
        }
    }
}
