using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tesseract;
using TranslateAppWPF.MVVM.Theme;

namespace TranslateAppWPF.MVVM.View
{
    public partial class NewView : UserControl
    {
        private readonly string directoryPath = "dictionaries";
        private string filePath;
        private string filename;

        private Dictionary<string, string> dictionary;

        public NewView()
        {
            InitializeComponent();
            Loaded += NewView_Loaded;

            KeyTextBox.LostFocus += TextBox_LostFocus;
            ValueTextBox.LostFocus += TextBox_LostFocus;

            dictionary = Json.LoadDictionaryFromFile(filePath);


            Add.Visibility = Visibility.Collapsed;
            OCR.Visibility = Visibility.Collapsed;
            KeyTextBox.Visibility = Visibility.Collapsed;
            ValueTextBox.Visibility = Visibility.Collapsed;
        }

        private void NewView_Loaded(object sender, RoutedEventArgs e)
        {
            NameTextBox.Focus();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        private void SaveDictionaryToFile()
        {
            Json.SaveDictionaryToFile(filePath, dictionary);
        }

        private void AddEntryButton_Click(object sender, RoutedEventArgs e)
        {
            string key = KeyTextBox.Text;
            string value = ValueTextBox.Text;

            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
            {
                if (dictionary == null)
                {
                    MessageBox.Show("Taki zestaw nie istnieje!");
                    return;
                }

                dictionary[key] = value;

                SaveDictionaryToFile();

                KeyTextBox.Text = null;
                ValueTextBox.Text = null;
            }
            else
            {
                MessageBox.Show("Proszę wprowadzić słowo i jego tłumaczenie!");
            }
        }


        private void NewSetButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = NameTextBox.Text;

            if (!string.IsNullOrEmpty(fileName))
            {
                filePath = Path.Combine(directoryPath, fileName + ".json");

                if (File.Exists(filePath))
                {
                    MessageBox.Show("Taki zestaw już istnieje!");
                    return;
                }

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                dictionary = new Dictionary<string, string>();

                SaveDictionaryToFile();

                Add.Visibility = Visibility.Visible;
                OCR.Visibility = Visibility.Visible;
                KeyTextBox.Visibility = Visibility.Visible;
                ValueTextBox.Visibility = Visibility.Visible;
                New.Visibility = Visibility.Collapsed;
                NameTextBox.Visibility = Visibility.Collapsed;
                Label1.Content = "Dodawanie wpisów";
            }
            else
            {
                MessageBox.Show("Proszę wprowadzić nazwę zestawu!");
            }
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog opendlg = new Microsoft.Win32.OpenFileDialog();
            opendlg.Filter = "(*.png, *.jpg, *.jpeg)|*.png;*.jpg;*.jpeg";
            Nullable<bool> result = opendlg.ShowDialog();

            if (result == true)
            {
                this.filename = opendlg.FileName;
                OCRButton_Click(sender, e);
            }
        }

        private void OCRButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(filename))
            {
                MessageBox.Show("No file selected!");
                return;
            }

            try
            {
                var executablePath = System.AppDomain.CurrentDomain.BaseDirectory;
                var tessdataPath = Path.Combine(executablePath, "tessdata");

                using (var engine = new TesseractEngine(tessdataPath, "eng+pol", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(filename))
                    {
                        using (var page = engine.Process(img))
                        {
                            var text = page.GetText();
                            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                            string currentKey = null;
                            string currentValue = "";

                            foreach (var line in lines)
                            {
                                // Assume each new key starts with a word followed by '—'
                                if (Regex.IsMatch(line, @"^\w+ —"))
                                {
                                    if (!string.IsNullOrEmpty(currentKey))
                                    {
                                        // Save the previous key-value pair
                                        dictionary[currentKey] = currentValue.Trim();
                                    }
                                    // Reset for new key-value pair
                                    int index = line.IndexOf("—");
                                    currentKey = line.Substring(0, index).Trim();
                                    currentValue = line.Substring(index + 1).Trim();
                                }
                                else if (!string.IsNullOrEmpty(currentValue))
                                {
                                    // Continue appending to current value
                                    currentValue += " " + line.Trim();
                                }
                            }

                            // Ensure the last entry is added
                            if (!string.IsNullOrEmpty(currentKey))
                            {
                                dictionary[currentKey] = currentValue.Trim();
                            }
                        }
                    }
                }

                SaveDictionaryToFile();
                MessageBox.Show("OCR processing complete and saved!");
            }
            catch (TesseractException tex)
            {
                MessageBox.Show("OCR engine error: " + tex.Message);
            }
            catch (IOException ioex)
            {
                MessageBox.Show("File I/O error: " + ioex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unhandled error during OCR processing: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}");
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SaveDictionaryToFile();
        }
    }
}
