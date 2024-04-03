using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;


namespace TranslateAppWPF.MVVM.ViewModel
{
    class StudyViewModel
    {

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
            set
            {
                _selectedFile = value;
                LoadTranslationsFromSelectedFile();
            }
        }

        private Dictionary<string, string> _translations = new Dictionary<string, string>();

        public Dictionary<string, string> Translations
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

        private void LoadTranslationsFromSelectedFile()
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
                            Translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent)
                                           ?? new Dictionary<string, string>();
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