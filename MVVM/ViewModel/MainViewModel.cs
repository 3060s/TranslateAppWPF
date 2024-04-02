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
            CurrentView = new HomeViewModel();

            HomeViewCommand = new RelayCommand(o =>
            {
                CurrentView = new HomeViewModel();
            });

            StudyViewCommand = new RelayCommand(o =>
            {
                CurrentView = new StudyViewModel();
            });

            EditViewCommand = new RelayCommand(o =>
            {
                CurrentView = new EditViewModel();
            });

            NewViewCommand = new RelayCommand(o =>
            {
                CurrentView = new NewViewModel();
            });

            DeleteViewCommand = new RelayCommand(o =>
            {
                CurrentView = new DeleteViewModel();
            });
        }
    }
}
