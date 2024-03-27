using System;

namespace TranslateAppWPF.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {

        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand StudyViewCommand { get; set; }
        public RelayCommand EditViewCommand { get; set; }
        public RelayCommand NewViewCommand { get; set; }
        public RelayCommand DeleteViewCommand { get; set; }


        public HomeViewModel HomeVM { get; set; }
        public StudyViewModel StudyVM { get; set; }
        public EditViewModel EditVM { get; set; }
        public NewViewModel NewVM { get; set; }
        public DeleteViewModel DeleteVM { get; set; }


        private object _currentview;

        public object CurrentView
        {
            get {  return _currentview; }
            set
            {
                _currentview = value;
                OnPropertyChanged();
            }

        }


        public MainViewModel()
        {
            HomeVM = new HomeViewModel();
            StudyVM = new StudyViewModel();
            EditVM = new EditViewModel();
            NewVM = new NewViewModel();
            DeleteVM = new DeleteViewModel();


            CurrentView = HomeVM;

            HomeViewCommand = new RelayCommand(o =>
            {
                CurrentView = HomeVM;
            });

            StudyViewCommand = new RelayCommand(o =>
            {
                CurrentView = StudyVM;
            });

            EditViewCommand = new RelayCommand(o =>
            {
                CurrentView = EditVM;
            });

            NewViewCommand = new RelayCommand(o =>
            {
                CurrentView = NewVM;
            });

            DeleteViewCommand = new RelayCommand(o =>
            {
                CurrentView = DeleteVM;
            });
        }
    }
}
