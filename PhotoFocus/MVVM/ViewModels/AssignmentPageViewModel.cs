using PhotoFocus.MVVM.Models;
using PhotoFocus.MVVM.Views;
using PhotoFocus.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Storage;

namespace PhotoFocus.MVVM.ViewModels
{
    public class AssignmentPageViewModel : BaseViewModel
    {
        private ObservableCollection<Assignment> _assignments;
        public ObservableCollection<Assignment> Assignments
        {
            get => _assignments;
            set => SetProperty(ref _assignments, value);
        }

        public ICommand NavigateToCreateAssignmentCommand { get; }
        public ICommand SelectAssignmentCommand { get; }

        // Property that indicates if the current user is an admin.
        private bool _isAdmin;
        public bool IsAdmin
        {
            get => _isAdmin;
            set => SetProperty(ref _isAdmin, value);
        }

        public AssignmentPageViewModel()
        {
            LoadAssignments();
            NavigateToCreateAssignmentCommand = new Command(async () => await NavigateToCreateAssignment());
            SelectAssignmentCommand = new Command(async (assignment) => await SelectAssignment(assignment));

            // Load current user info to determine if they are an admin.
            LoadCurrentUser();
        }

        private async void LoadAssignments()
        {
            var assignments = await DatabaseService.Database.Table<Assignment>().ToListAsync();
            Assignments = new ObservableCollection<Assignment>(assignments);
        }

        private async Task LoadCurrentUser()
        {
            // Retrieve the current user ID from SecureStorage.
            var storedUserId = await SecureStorage.GetAsync("userId");
            if (!string.IsNullOrEmpty(storedUserId) && int.TryParse(storedUserId, out int id))
            {
                var user = await DatabaseService.Database.Table<User>()
                                 .Where(u => u.Id == id)
                                 .FirstOrDefaultAsync();
                if (user != null)
                {
                    IsAdmin = user.IsAdmin;
                }
            }
        }

        private async Task NavigateToCreateAssignment()
        {
            // Navigate to CreateAssignmentPage (only accessible if admin)
            await App.Current.MainPage.Navigation.PushAsync(new CreateAssignmentPage());
        }

        private async Task SelectAssignment(object assignmentObj)
        {
            if (assignmentObj is Assignment assignment)
            {
                // Create a new UploadPhotoViewModel instance and set the selected assignment.
                var vm = new UploadPhotoViewModel
                {
                    SelectedAssignment = assignment
                };

                // Create the UploadPhotoPage and assign the view model.
                var uploadPhotoPage = new UploadPhotoPage { BindingContext = vm };
                await App.Current.MainPage.Navigation.PushAsync(uploadPhotoPage);
            }
        }
    }
}
