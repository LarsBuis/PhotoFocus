using PhotoFocus.MVVM.Models;
using PhotoFocus.MVVM.Views;
using PhotoFocus.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoFocus.MVVM.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;

        private ObservableCollection<Category> _filterCategories;
        public ObservableCollection<Category> FilterCategories
        {
            get => _filterCategories;
            set => SetProperty(ref _filterCategories, value);
        }

        private Category _selectedFilterCategory;
        public Category SelectedFilterCategory
        {
            get => _selectedFilterCategory;
            set
            {
                if (SetProperty(ref _selectedFilterCategory, value))
                {
                    LoadFilteredPhotos();
                }
            }
        }

        private string _filterTitle;
        public string FilterTitle
        {
            get => _filterTitle;
            set => SetProperty(ref _filterTitle, value);
        }

        private ObservableCollection<PhotoDisplayItem> _filteredPhotos;
        public ObservableCollection<PhotoDisplayItem> FilteredPhotos
        {
            get => _filteredPhotos;
            set => SetProperty(ref _filteredPhotos, value);
        }

        private List<Photo> _allPhotos;
        private Category _allCategory = new Category { Id = -1, Name = "All Photos" };

        // Commands
        public ICommand FilterCommand { get; }
        public ICommand AddPhotoCommand { get; }

        public HomeViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            _allPhotos = new List<Photo>();
            FilteredPhotos = new ObservableCollection<PhotoDisplayItem>();
            FilterCategories = new ObservableCollection<Category>();

            LoadData();

            FilterCommand = new Command(async () => await OnFilterClicked());
            AddPhotoCommand = new Command(async () =>
            {
                await _navigationService.NavigateToUploadPhotoAsync();
            });
        }

        private async void LoadData()
        {
            var cats = await DatabaseService.Database.Table<Category>().ToListAsync();
            FilterCategories.Add(_allCategory);
            foreach (var cat in cats)
            {
                FilterCategories.Add(cat);
            }

            _allPhotos = await DatabaseService.Database.Table<Photo>().ToListAsync();

            SelectedFilterCategory = _allCategory;
        }

        private async void LoadFilteredPhotos()
        {
            if (SelectedFilterCategory == null || SelectedFilterCategory.Id == -1)
            {
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
            FilteredPhotos.Clear();
            foreach (var p in photos)
            {
                var user = await DatabaseService.Database.Table<User>()
                    .Where(u => u.Id == p.UserId)
                    .FirstOrDefaultAsync();

                var category = FilterCategories.FirstOrDefault(c => c.Id == p.CategoryId);

                var item = new PhotoDisplayItem
                {
                    Photo = p,
                    User = user,
                    Category = category
                };

                await item.InitializeAsync();
                FilteredPhotos.Add(item);
            }
        }

        private async Task OnFilterClicked()
        {
            var categoryNames = FilterCategories.Select(c => c.Name).ToArray();
            var action = await Application.Current.MainPage.DisplayActionSheet(
                "Choose a category",
                "Cancel",
                null,
                categoryNames
            );

            if (action == "Cancel" || string.IsNullOrEmpty(action))
                return;

            var chosenCategory = FilterCategories.FirstOrDefault(c => c.Name == action);
            if (chosenCategory != null)
            {
                SelectedFilterCategory = chosenCategory;
            }
        }
    }

    public class PhotoDisplayItem : BaseViewModel
    {
        private int _likeCount;
        public int LikeCount
        {
            get => _likeCount;
            set => SetProperty(ref _likeCount, value);
        }

        private bool _hasLiked;
        public bool HasLiked
        {
            get => _hasLiked;
            set => SetProperty(ref _hasLiked, value);
        }

        public Photo Photo { get; set; }
        public User User { get; set; }
        public Category Category { get; set; }

        public ICommand ToggleLikeCommand { get; }

        public PhotoDisplayItem()
        {
            ToggleLikeCommand = new Command(async () => await OnToggleLikeAsync());
        }

        public async Task InitializeAsync()
        {
            if (Photo == null)
                return;

            int currentUserId = await GetCurrentUserIdAsync();

            LikeCount = await DatabaseService.GetLikeCountAsync(Photo.Id);

            // check if liked
            var existingLike = await DatabaseService.Database.Table<PhotoLike>()
                .Where(l => l.PhotoId == Photo.Id && l.UserId == currentUserId)
                .FirstOrDefaultAsync();

            HasLiked = (existingLike != null);
        }

        private async Task OnToggleLikeAsync()
        {
            if (Photo == null)
                return;

            int currentUserId = await GetCurrentUserIdAsync();

            bool isNowLiked = await DatabaseService.ToggleLikeAsync(Photo.Id, currentUserId);

            LikeCount = await DatabaseService.GetLikeCountAsync(Photo.Id);
            HasLiked = isNowLiked;
        }

        private async Task<int> GetCurrentUserIdAsync()
        {
            var storedUserId = await SecureStorage.GetAsync("userId");
            if (!string.IsNullOrEmpty(storedUserId) && int.TryParse(storedUserId, out int id))
                return id;
            else
                return 0;
        }
    }
}
