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
            set { _translations = value; }
        }

        public RelayCommand LoadTranslationsCommand { get; private set; }

        public StudyViewModel()
        {
            LoadTranslationsCommand = new RelayCommand(_ => LoadTranslationsFromSelectedFile());
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
                            IsSetListVisible = true;
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