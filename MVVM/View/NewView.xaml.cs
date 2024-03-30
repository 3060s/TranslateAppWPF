using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TranslateAppWPF.MVVM.Theme;

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

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                dictionary = new Dictionary<string, string>();

                SaveDictionaryToFile();

                Add.Visibility = Visibility.Visible;
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

        private void Window_Closed(object sender, EventArgs e)
        {
            SaveDictionaryToFile();
        }
    }
}
