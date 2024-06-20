using System.Windows;
using System.Windows.Controls;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;

namespace pogodynka
{
    public static class PlaceholderService
    {
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.RegisterAttached(
                "Placeholder",
                typeof(string),
                typeof(PlaceholderService),
                new PropertyMetadata(string.Empty, OnPlaceholderChanged));

        public static string GetPlaceholder(TextBox textBox)
        {
            return (string)textBox.GetValue(PlaceholderProperty);
        }

        public static void SetPlaceholder(TextBox textBox, string value)
        {
            textBox.SetValue(PlaceholderProperty, value);
        }

        private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                textBox.TextChanged -= OnTextChanged;
                textBox.GotFocus -= OnGotFocus;
                textBox.LostFocus -= OnLostFocus;

                if (!string.IsNullOrEmpty(GetPlaceholder(textBox)))
                {
                    textBox.TextChanged += OnTextChanged;
                    textBox.GotFocus += OnGotFocus;
                    textBox.LostFocus += OnLostFocus;

                    UpdatePlaceholder(textBox);
                }
            }
        }

        private static void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePlaceholder(sender as TextBox);
        }

        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Text == GetPlaceholder(textBox))
            {
                textBox.Text = string.Empty;
                textBox.Foreground = SystemColors.ControlTextBrush;
            }
        }

        private static void OnLostFocus(object sender, RoutedEventArgs e)
        {
            UpdatePlaceholder(sender as TextBox);
        }

        private static void UpdatePlaceholder(TextBox textBox)
        {
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = GetPlaceholder(textBox);
                textBox.Foreground = SystemColors.GrayTextBrush;
            }
            else if (textBox.Text == GetPlaceholder(textBox))
            {
                textBox.Foreground = SystemColors.GrayTextBrush;
            }
            else
            {
                textBox.Foreground = SystemColors.ControlTextBrush;
            }
        }
    }
}
