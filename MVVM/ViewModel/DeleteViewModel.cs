using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace TranslateAppWPF.MVVM.ViewModel;

class DeleteViewModel
{
    private ObservableCollection<string> _files = [];

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

    public RelayCommand DeleteCommand { get; private set; }

    public DeleteViewModel()
    {
        RefreshFiles();

        DeleteCommand = new(_ =>
        {
            DeleteFile();
        }, _ => !string.IsNullOrEmpty(SelectedFile));
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

    private void DeleteFile()
    {
        string directoryPath = "dictionaries";
        string filePath = Path.Combine(directoryPath, SelectedFile);
        try
        {
            File.Delete(filePath);
            MessageBox.Show($"Zestaw '{SelectedFile}' został pomyślnie usunięty!");
            Files.Remove(SelectedFile);
            SelectedFile = null;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Podczas usuwania zestawu wystąpił problem: {ex.Message}");
        }
    }
}