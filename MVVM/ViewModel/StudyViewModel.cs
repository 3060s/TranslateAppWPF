using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Newtonsoft.Json;


namespace TranslateAppWPF.MVVM.ViewModel
{
    class StudyViewModel : ObservableObject
    {

        private DispatcherTimer _timer;

        private int _timerValue;

        public int TimerValue
        {
            get { return _timerValue; }
            set
            {
                _timerValue = value;
                OnPropertyChanged();
            }
        }


        private string _timerValueString;

        public string TimerValueString
        {
            get { return _timerValueString; }
            set
            {
                _timerValueString = value;
                OnPropertyChanged();
            }
        }

        private double _avgAnswerTime;

        public double AvgAnswerTime
        {
            get { return _avgAnswerTime; }
            set
            {
                _avgAnswerTime = value;
                OnPropertyChanged();
            }
        }


        private string _avgAnswerTimeFormatted;

        public string AvgAnswerTimeFormatted
        {
            get { return _avgAnswerTimeFormatted; }
            set
            {
                _avgAnswerTimeFormatted = value;
                OnPropertyChanged();
            }
        }


        private bool _isLabelWordVisible;

        public bool IsLabelWordVisible
        {
            get { return _isLabelWordVisible; }
            set
            {
                _isLabelWordVisible = value;
                OnPropertyChanged();
            }
        }


        private bool _isLabel1Visible;

        public bool IsLabel1Visible
        {
            get { return _isLabel1Visible; }
            set
            {
                _isLabel1Visible = value;
                OnPropertyChanged();
            }
        }


        private bool _isWordCorrectnessVisible;

        public bool IsWordCorrectnessVisible
        {
            get { return _isWordCorrectnessVisible; }
            set
            {
                _isWordCorrectnessVisible = value;
                OnPropertyChanged();
            }
        }


        private bool _isAnswerCounterVisible;

        public bool IsAnswerCounterVisible
        {
            get { return _isAnswerCounterVisible; }
            set
            {
                _isAnswerCounterVisible = value;
                OnPropertyChanged();
            }
        }


        private bool _isSetListVisible;

        public bool IsSetListVisible
        {
            get { return _isSetListVisible; }
            set
            {
                _isSetListVisible = value;
                OnPropertyChanged();
            }
        }


        private bool _isButtonVisible;

        public bool IsButtonVisible
        {
            get { return _isButtonVisible; }
            set
            {
                _isButtonVisible = value;
                OnPropertyChanged();
            }
        }


        private bool _isSummaryButtonVisible;

        public bool IsSummaryButtonVisible
        {
            get { return _isSummaryButtonVisible; }
            set
            {
                _isSummaryButtonVisible = value;
                OnPropertyChanged();
            }
        }


        private bool _isSummaryVisible;

        public bool IsSummaryVisible
        {
            get { return _isSummaryVisible; }
            set
            {
                _isSummaryVisible = value;
                OnPropertyChanged();
            }
        }


        private bool _isComboBoxVisible;

        public bool IsComboBoxVisible
        {
            get { return _isComboBoxVisible; }
            set
            {
                _isComboBoxVisible = value;
                OnPropertyChanged();
            }
        }


        private bool _isInputBoxVisible;

        public bool IsInputBoxVisible
        {
            get { return _isInputBoxVisible; }
            set
            {
                _isInputBoxVisible = value;
                OnPropertyChanged();
            }
        }


        private ObservableCollection<string> _files = new ObservableCollection<string>();

        public ObservableCollection<string> Files
        {
            get { return _files; }
            set { _files = value; }
        }

        private string? _selectedFile = null;

        public string? SelectedFile
        {
            get { return _selectedFile; }
            set { _selectedFile = value; }
        }

        private ObservableCollection<KeyValuePair<string, string>> _translations = [];

        public ObservableCollection<KeyValuePair<string, string>> Translations
        {
            get { return _translations; }
            set
            {
                _translations = value;
                OnPropertyChanged();
            }
        }


        private string _randomLabelText = "Default Label Text";

        public string RandomLabelText
        {
            get { return _randomLabelText; }
            set
            {
                _randomLabelText = value;
                OnPropertyChanged();
            }
        }


        private string _userTranslation = "";

        public string UserTranslation
        {
            get { return _userTranslation; }
            set
            {
                _userTranslation = value;
                OnPropertyChanged();
            }
        }


        private string _wordCorrectness = "";

        public string WordCorrectness
        {
            get { return _wordCorrectness; }
            set
            {
                _wordCorrectness = value;
                OnPropertyChanged();
            }
        }


        private int _correctAnswersCount = 0;
        private int _wrongAnswersCount = 0;
        private int _totalAnswersCount = 0;

        private string _totalAnswers;

        public string TotalAnswers
        {
            get { return _totalAnswers; }
            set
            {
                _totalAnswers = value;
                OnPropertyChanged();
            }
        }


        private string _wrongAnswers;

        public string WrongAnswers
        {
            get { return _wrongAnswers; }
            set
            {
                _wrongAnswers = value;
                OnPropertyChanged();
            }
        }


        private string _correctAnswers;

        public string CorrectAnswers
        {
            get { return _correctAnswers; }
            set
            {
                _correctAnswers = value;
                OnPropertyChanged();
            }
        }


        private SolidColorBrush _wordCorrectnessColor = new SolidColorBrush(Colors.Black);

        public SolidColorBrush WordCorrectnessColor
        {
            get { return _wordCorrectnessColor; }
            set
            {
                _wordCorrectnessColor = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand LoadTranslationsCommand { get; private set; }
        public RelayCommand RandomizeLabelTextCommand { get; private set; }
        public RelayCommand CheckTranslationCommand { get; private set; }
        public RelayCommand SummaryCommand { get; private set; }

        public StudyViewModel()
        {
            LoadTranslationsCommand = new RelayCommand(_ => LoadTranslationsFromSelectedFile());
            RandomizeLabelTextCommand = new RelayCommand(_ => RandomizeLabelText());
            CheckTranslationCommand = new RelayCommand(_ => CheckTranslation());
            SummaryCommand = new RelayCommand(_ => Summary());

            IsLabelWordVisible = false;
            IsLabel1Visible = true;
            IsWordCorrectnessVisible = false;
            IsAnswerCounterVisible = false;
            IsSetListVisible = true;
            IsButtonVisible = true;
            IsComboBoxVisible = true;
            IsInputBoxVisible = false;
            IsSummaryButtonVisible = false;


            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += TimerTick;

            RefreshFiles();
        }


        public void LoadTranslationsFromSelectedFile()
        {
            Translations.Clear();

            if (!string.IsNullOrEmpty(SelectedFile))
            {
                string filePath = Path.Combine("dictionaries", SelectedFile);
                if (File.Exists(filePath))
                {
                    try
                    {
                        string jsonContent = File.ReadAllText(filePath);
                        if (!string.IsNullOrEmpty(jsonContent))
                        {
                            var translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent) ?? [];
                            IsSetListVisible = true;
                            foreach (var translation in translations)
                            {
                                Translations.Add(translation);
                            }

                            IsLabelWordVisible = true;
                            IsLabel1Visible = false;
                            IsWordCorrectnessVisible = true;
                            IsSetListVisible = false;
                            IsButtonVisible = false;
                            IsComboBoxVisible = false;
                            IsInputBoxVisible = true;
                            IsSummaryButtonVisible = true;

                            _timer.Start();

                            RandomizeLabelText();
                        }
                        else
                        {
                            MessageBox.Show("The JSON content is empty.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading translations: {ex.Message}");
                    }
                }
            }
        }

        private void RandomizeLabelText()
        {
            if (Translations.Count > 0)
            {
                Random random = new Random();
                int randomIndex = random.Next(0, Translations.Count);
                var randomTranslation = Translations[randomIndex];
                RandomLabelText = $"Przetłumacz: {randomTranslation.Key}";
            }
            else
            {
                RandomLabelText = "Nie załadowano tłumaczeń.";
            }
        }

        private void CheckTranslation()
        {
            if (Translations.Count > 0)
            {
                var randomTranslation = Translations.FirstOrDefault(t => t.Key == RandomLabelText.Replace("Przetłumacz: ", ""));

                if (randomTranslation.Key != null)
                {
                    string userInput = UserTranslation.Trim();
                    _totalAnswersCount++;

                    if (string.Equals(userInput, randomTranslation.Value, StringComparison.OrdinalIgnoreCase))
                    {
                        RandomizeLabelText();
                        UserTranslation = "";
                        WordCorrectnessColor = new SolidColorBrush(Colors.Green);
                        WordCorrectness = "Dobrze!";
                        _correctAnswersCount++;
                    }
                    else
                    {
                        RandomizeLabelText();
                        UserTranslation = "";
                        WordCorrectnessColor = new SolidColorBrush(Colors.Red);
                        WordCorrectness = "Źle!";
                        _wrongAnswersCount++;
                    }
                }
                else
                {
                    MessageBox.Show("Nie znaleziono tłumaczenia dla podanego klucza.");
                }
            }
            else
            {
                MessageBox.Show("Nie załadowano tłumaczeń.");
            }
        }

        public void RefreshFiles()
        {
            Files.Clear();

            string directoryPath = "dictionaries";
            if (Directory.Exists(directoryPath))
            {
                string[] jsonFiles = Directory.GetFiles(directoryPath, "*.json");
                foreach (string jsonFile in jsonFiles)
                {
                    Files.Add(Path.GetFileName(jsonFile));
                }
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            TimerValue++;
        }

        private void Summary()
        {
            _timer.Stop();
            IsLabelWordVisible = false;
            IsWordCorrectnessVisible = false;
            IsAnswerCounterVisible = true;
            IsInputBoxVisible = false;
            IsSummaryButtonVisible = false;
            IsSummaryVisible = true;

            _avgAnswerTime = (_totalAnswersCount > 0) ? (double)TimerValue / _totalAnswersCount : 0;

            TotalAnswers = $"Ilość odpowiedzi: {_totalAnswersCount}";
            CorrectAnswers = $"Dobre odpowiedzi: {_correctAnswersCount}";
            WrongAnswers = $"Złe odpowiedzi: {_wrongAnswersCount} ";
            TimerValueString = $"Łączny czas nauki: {TimerValue}s";
            AvgAnswerTimeFormatted = $"Średni czas na odpowiedź: {_avgAnswerTime.ToString("0.00")}s";
        }
    }
}