using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TranslateAppWPF.MVVM.View
{
    public partial class NewView : UserControl
    {
        private readonly string directoryPath = @"D:\vs2022\TranslateAppWPF\dictionaries";
        private string filePath;
        private Dictionary<string, string> dictionary;

        public NewView()
        {
            InitializeComponent();
            Loaded += NewView_Loaded;

            KeyTextBox.LostFocus += TextBox_LostFocus;
            ValueTextBox.LostFocus += TextBox_LostFocus;

            Loaded += (sender, e) =>
            {
                Window window = Window.GetWindow(this);
                if (window != null)
                {
                    window.PreviewMouseDown += Window_PreviewMouseDown;
                }
            };

            dictionary = Json.LoadDictionaryFromFile(filePath);


            Add.Visibility = Visibility.Collapsed;
            KeyTextBox.Visibility = Visibility.Collapsed;
            ValueTextBox.Visibility = Visibility.Collapsed;
        }

        private void NewView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(e.Source is TextBox))
            {
                Keyboard.ClearFocus();
            }
        }

        private void SaveDictionaryToFile()
        {
            Json.SaveDictionaryToFile(filePath, dictionary);
            MessageBox.Show($"Dictionary saved to {filePath}");
        }

        private void AddEntryButton_Click(object sender, RoutedEventArgs e)
        {
            string key = KeyTextBox.Text;
            string value = ValueTextBox.Text;
            string fileName = NameTextBox.Text;

            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(fileName))
            {
                if (dictionary == null)
                {
                    dictionary = new Dictionary<string, string>();
                }

                dictionary[key] = value;

                filePath = Path.Combine(directoryPath, fileName + ".json");

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                SaveDictionaryToFile();
            }
            else
            {
                MessageBox.Show("Please enter key, value, and file name!");
            }
        }


        private void NewSetButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SaveDictionaryToFile();
        }
    }
}
