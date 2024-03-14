using System;
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
using TranslateAppWPF.windows;

namespace TranslateAppWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }

        private void Study_Click(object sender, RoutedEventArgs e)
        {
            Study studyWindow = new Study();
            studyWindow.Show();
            this.Close();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Edit editWindow = new Edit();
            editWindow.Show();
            this.Close();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            New newWindow = new New();
            newWindow.Show();
            this.Close();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Delete deleteWindow = new Delete();
            deleteWindow.Show();
            this.Close();
        }
    }
}