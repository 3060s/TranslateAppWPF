using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace TranslateAppWPF.MVVM.View
{
    public partial class DeleteView : UserControl
    {
        private string selectedFileName;

        public DeleteView()
        {
            InitializeComponent();

            PopulateJsonFilesComboBox();
        }

        private void PopulateJsonFilesComboBox()
        {
            string directoryPath = @"D:\vs2022\TranslateAppWPF\dictionaries";
            if (Directory.Exists(directoryPath))
            {
                string[] jsonFiles = Directory.GetFiles(directoryPath, "*.json");
                foreach (string jsonFile in jsonFiles)
                {
                    jsonFilesComboBox.Items.Add(Path.GetFileName(jsonFile));
                }
            }
            else
            {
                MessageBox.Show("Taka ścieżka nie istnieje!");
            }
        }

        private void JsonFilesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedFileName = jsonFilesComboBox.SelectedItem as string;
        }

        private void DeleteSelectedJsonFile_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedFileName))
            {
                string directoryPath = @"D:\vs2022\TranslateAppWPF\dictionaries";
                string filePath = Path.Combine(directoryPath, selectedFileName);
                try
                {
                    File.Delete(filePath);
                    MessageBox.Show($"Zestaw '{selectedFileName}' został pomyślnie usunięty!");
                    jsonFilesComboBox.Items.Remove(selectedFileName);
                    selectedFileName = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Podczas usuwania zestawu wystąpił problem: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Proszę wybierz zestaw do usunięcia");
            }
        }
    }
}