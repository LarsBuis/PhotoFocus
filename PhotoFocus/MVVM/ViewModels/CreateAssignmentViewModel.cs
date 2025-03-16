using PhotoFocus.MVVM.Models;
using PhotoFocus.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PhotoFocus.MVVM.ViewModels
{
    public class CreateAssignmentViewModel : BaseViewModel
    {
        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        // Options for assignment duration
        public ObservableCollection<string> DurationOptions { get; set; }

        private string _selectedDuration;
        public string SelectedDuration
        {
            get => _selectedDuration;
            set => SetProperty(ref _selectedDuration, value);
        }

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public ICommand CreateAssignmentCommand { get; }

        public CreateAssignmentViewModel()
        {
            // Predefined duration options (you can add more if needed)
            DurationOptions = new ObservableCollection<string> { "7 days", "2 weeks" };
            SelectedDuration = DurationOptions[0];

            CreateAssignmentCommand = new Command(CreateAssignment);
        }

        private async void CreateAssignment()
        {
            // Determine duration in days based on the selected option
            int durationDays = SelectedDuration == "2 weeks" ? 14 : 7;

            var assignment = new Assignment
            {
                Title = this.Title,
                Description = this.Description,
                StartDate = DateTime.Now,
                DurationDays = durationDays
            };

            // Save to the database (assumes DatabaseService.Database is set up for the Assignment table)
            var result = await DatabaseService.Database.InsertAsync(assignment);
            if (result > 0)
            {
                Message = "Assignment created successfully!";
                // Optionally clear the fields:
                Title = "";
                Description = "";
            }
            else
            {
                Message = "Failed to create assignment.";
            }
        }
    }
}
