﻿using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.IO;

namespace TranslateAppWPF
{
    public partial class MainWindow : Window
    {
        private readonly string directoryPath = @"D:\vs2022\TranslateAppWPF\dictionaries";
        private string filePath;
        private Dictionary<string, string> dictionary;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;

            dictionary = Json.LoadDictionaryFromFile(filePath);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
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

                filePath = System.IO.Path.Combine(directoryPath, fileName + ".json");

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

        private void RemoveEntryButton_Click(object sender, RoutedEventArgs e)
        {
            string searchText = RemoveTextBox.Text;

            if (dictionary == null || dictionary.Count == 0)
            {
                MessageBox.Show("Dictionary is empty.");
                return;
            }

            if (string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Please enter search text.");
                return;
            }

            KeyValuePair<string, string>? entryToRemove = null;
            foreach (var entry in dictionary)
            {
                if (entry.Key == searchText || entry.Value == searchText)
                {
                    entryToRemove = entry;
                    break;
                }
            }

            if (entryToRemove != null)
            {
                dictionary.Remove(entryToRemove.Value.Key);
                SaveDictionaryToFile();
                MessageBox.Show("Entry removed successfully.");
            }
            else
            {
                MessageBox.Show("Entry not found.");
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SaveDictionaryToFile();
        }
    }
}