using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TranslateAppWPF.MVVM.Theme
{
    public static class WatermarkService
    {
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(WatermarkService), new PropertyMetadata(OnWatermarkChanged));

        public static string GetWatermark(DependencyObject obj)
        {
            return (string)obj.GetValue(WatermarkProperty);
        }

        public static void SetWatermark(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkProperty, value);
        }

        private static void OnWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if ((string)e.NewValue != string.Empty)
                {
                    textBox.GotFocus += OnTextBoxGotFocus;
                    textBox.LostFocus += OnTextBoxLostFocus;
                    textBox.TextChanged += OnTextBoxTextChanged;
                    SetWatermarkText(textBox);
                }
            }
        }

        private static void OnTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (GetWatermark(textBox) == textBox.Text)
            {
                textBox.Text = string.Empty;
                textBox.FontStyle = FontStyles.Normal;
            }
        }

        private static void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                SetWatermarkText(textBox);
            }
            else
            {
                textBox.Foreground = Brushes.Black;
            }
        }

        private static void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (textBox.Foreground != Brushes.Black)
            {
                textBox.Foreground = Brushes.Black;
            }
        }

        private static void SetWatermarkText(TextBox textBox)
        {
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = GetWatermark(textBox);
                textBox.FontStyle = FontStyles.Italic;
                textBox.Foreground = Brushes.Gray;

                switch (textBox.Name)
                {
                    case "NameTextBox":
                        textBox.FontSize = 28;
                        break;
                    case "KeyTextBox":
                    case "ValueTextBox":
                        textBox.FontSize = 18;
                        break;
                    default:
                        textBox.FontSize = 12;
                        break;
                }
            }
        }
    }
}