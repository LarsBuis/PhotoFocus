using PhotoFocus.MVVM.Models;
using PhotoFocus.MVVM.Views;
using PhotoFocus.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

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

        public AssignmentPageViewModel()
        {
            LoadAssignments();
            NavigateToCreateAssignmentCommand = new Command(async () => await NavigateToCreateAssignment());
        }

        private async void LoadAssignments()
        {
            var assignments = await DatabaseService.Database.Table<Assignment>().ToListAsync();
            Assignments = new ObservableCollection<Assignment>(assignments);
        }

        private async Task NavigateToCreateAssignment()
        {
            // Navigate to the CreateAssignmentPage
            await App.Current.MainPage.Navigation.PushAsync(new CreateAssignmentPage());
        }
    }
}
