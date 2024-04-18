using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Newtonsoft.Json;


namespace TranslateAppWPF.MVVM.ViewModel
{
    class StudyViewModel : ObservableObject
    {

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

        public RelayCommand LoadTranslationsCommand { get; private set; }
        public RelayCommand RandomizeLabelTextCommand { get; private set; }
        public RelayCommand CheckTranslationCommand { get; private set; }

        public StudyViewModel()
        {
            LoadTranslationsCommand = new RelayCommand(_ => LoadTranslationsFromSelectedFile());
            RandomizeLabelTextCommand = new RelayCommand(_ => RandomizeLabelText());
            CheckTranslationCommand = new RelayCommand(_ => CheckTranslation());


            IsSetListVisible = true;
            IsButtonVisible = true;
            IsComboBoxVisible = true;
            IsInputBoxVisible = true; //

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
                            IsSetListVisible = false;
                            IsButtonVisible = false;
                            IsComboBoxVisible = false;
                            IsInputBoxVisible = true;

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

                    if (string.Equals(userInput, randomTranslation.Value, StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("Poprawne tłumaczenie!");
                        RandomizeLabelText();
                    }
                    else
                    {
                        MessageBox.Show("Złe tłumaczenie!");
                        RandomizeLabelText();
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
    }
}