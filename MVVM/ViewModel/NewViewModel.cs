using Microsoft.ML;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Tesseract;
using TranslateAppWPF.MachineLearning;


namespace TranslateAppWPF.MVVM.ViewModel
{
    class NewViewModel : ObservableObject
    {
        private MLContext mlContext;
        private PredictionEngine<OcrModelInput, OcrModelOutput> predEngine;
        private Dictionary<string, string> dictionary;

        private readonly string directoryPath = "dictionaries";
        private string filePath;
        private string filename;

        private string _name;
        private string _key;
        private string _value;


        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }


        public string Key
        {
            get => _key;
            set
            {
                _key = value;
                OnPropertyChanged();
            }
        }


        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }


        private string _filename;

        public string FileName
        {
            get => _filename;
            set
            {
                if (_filename != value)
                {
                    _filename = value;
                    OnPropertyChanged();
                }
            }
        }


        private Visibility _setCreationVisibility = Visibility.Visible;

        public Visibility SetCreationVisibility
        {
            get => _setCreationVisibility;
            set
            {
                _setCreationVisibility = value;
                OnPropertyChanged();
            }
        }


        private Visibility _entryCreationVisibility = Visibility.Collapsed;

        public Visibility EntryCreationVisibility
        {
            get => _entryCreationVisibility;
            set
            {
                _entryCreationVisibility = value;
                OnPropertyChanged();
            }
        }



        public ICommand AddEntryCommand { get; private set; }
        public ICommand NewSetCommand { get; private set; }
        public ICommand ProcessOCRCommand { get; private set; }
        public ICommand OpenFileCommand { get; private set; }



        public NewViewModel()
        {
            mlContext = new MLContext();
            //InitializeMLComponents();


            dictionary = Json.LoadDictionaryFromFile(filePath);

            AddEntryCommand = new RelayCommand(obj => AddEntry());
            NewSetCommand = new RelayCommand(obj => CreateNewSet());
            ProcessOCRCommand = new RelayCommand(obj => ProcessOCR());
            OpenFileCommand = new RelayCommand(obj => OpenFile());
        }



        private void InitializeMLComponents()
        {
            // Assuming model is already trained and serialized at a known path
            var modelPath = "path_to_trained_model.zip";
            ITransformer loadedModel = mlContext.Model.Load(modelPath, out var modelSchema);
            predEngine = mlContext.Model.CreatePredictionEngine<OcrModelInput, OcrModelOutput>(loadedModel);
        }


        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
        }


        private void SaveDictionaryToFile()
        {
            Json.SaveDictionaryToFile(filePath, dictionary);
        }


        private void AddEntry()
        {
            if (dictionary == null)
            {
                MessageBox.Show("Taki zestaw nie istnieje!");
                return;
            }

            if (string.IsNullOrEmpty(Key) || string.IsNullOrEmpty(Value))
            {
                MessageBox.Show("Proszę wpisać słowo i jego tłumaczenie!");
                return;
            }

            if (dictionary.ContainsKey(Key))
            {
                MessageBox.Show("Taki wpis już istnieje!");
                return;
            }

            dictionary[Key] = Value;
            SaveDictionaryToFile();

            Key = string.Empty;
            Value = string.Empty;

            MessageBox.Show("Poprawnie dodano wpis!");
        }


        private void CreateNewSet()
        {

            if (string.IsNullOrEmpty(Name))
            {
                MessageBox.Show("Proszę wprowadzić nazwę zestawu!");
                return;
            }

            filePath = Path.Combine(directoryPath, Name + ".json");
            if (File.Exists(filePath))
            {
                MessageBox.Show("Taki zestaw już istnieje!");
                return;
            }

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            SetCreationVisibility = Visibility.Collapsed;
            EntryCreationVisibility = Visibility.Visible;
            dictionary = new Dictionary<string, string>();
            SaveDictionaryToFile();
        }


        private void OpenFile()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.png; *.jpg; *.jpeg)|*.png;*.jpg;*.jpeg"
            };

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                FileName = openFileDialog.FileName;
                ProcessOCR().ConfigureAwait(false);
            }
        }


        public async Task ProcessOCR()
        {
            if (string.IsNullOrEmpty(FileName))
            {
                MessageBox.Show("Nie wybrano pliku!");
                return;
            }

            try
            {
                var tessdataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
                using (var engine = new TesseractEngine(tessdataPath, "eng+pol", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(FileName))
                    {
                        using (var page = engine.Process(img))
                        {
                            var text = page.GetText();
                            await ProcessText(text);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("OCR processing error: " + ex.Message);
            }
        }


        private async Task ProcessText(string text)
        {
            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string currentKey = string.Empty;
            string currentValue = string.Empty;

            foreach (var line in lines)
            {
                if (Regex.IsMatch(line, @"^\w+ —"))
                {
                    if (!string.IsNullOrEmpty(currentKey))
                    {
                        dictionary[currentKey] = currentValue.Trim();
                    }
                    int index = line.IndexOf("—");
                    currentKey = line.Substring(0, index).Trim();
                    currentValue = line.Substring(index + 1).Trim();
                }
                else if (!string.IsNullOrEmpty(currentValue))
                {
                    currentValue += " " + line.Trim();
                }
            }

            if (!string.IsNullOrEmpty(currentKey))
            {
                dictionary[currentKey] = currentValue.Trim();
            }

            SaveDictionaryToFile();

            MessageBox.Show("Proces OCR zakończył się powodzeniem!");
        }
    }
}