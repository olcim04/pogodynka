using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace pogodynka
{
    public partial class MainWindow : Window
    {
        private const string ApiKey = "7bf63a2665f7271cf817fe1c9fd9be35"; 

        public MainWindow()
        {
            InitializeComponent();
            CityComboBox.SelectionChanged += CityComboBox_SelectionChanged;
        }

        private async void GetWeatherButton_Click(object sender, RoutedEventArgs e)
        {
            string cityName = CityTextBox.Text;
            if (string.IsNullOrWhiteSpace(cityName))
            {
                if (CityComboBox.SelectedItem != null && CityComboBox.SelectedIndex != 0)
                {
                    cityName = ((ComboBoxItem)CityComboBox.SelectedItem).Content.ToString();
                    CityTextBox.Text = cityName; // Automatycznie wprowadza miasto do TextBoxa
                }
                else
                {
                    MessageBox.Show("Proszę wprowadzić nazwę miasta lub wybrać miasto z listy.");
                    return;
                }
            }

            await GetWeatherData(cityName);
        }

        private async void CityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedIndex > 0)
            {
                string cityName = ((ComboBoxItem)comboBox.SelectedItem).Content.ToString();
                CityTextBox.Text = cityName; // Automatycznie wprowadza miasto do TextBoxa
                await GetWeatherData(cityName);
            }
        }

        private async Task GetWeatherData(string cityName)
        {
            try
            {
                string url = $"http://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={ApiKey}&units=metric";

                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(url);
                        response.EnsureSuccessStatusCode();

                        string responseBody = await response.Content.ReadAsStringAsync();
                        JObject weatherData = JObject.Parse(responseBody);

                        double temperature = weatherData["main"]["temp"].Value<double>();
                        int pressure = weatherData["main"]["pressure"].Value<int>();
                        int humidity = weatherData["main"]["humidity"].Value<int>();
                        string description = weatherData["weather"][0]["description"].Value<string>();
                        string icon = weatherData["weather"][0]["icon"].Value<string>();

                        WeatherInfoTextBlock.Text = $"Temperatura: {temperature}°C\nCiśnienie: {pressure} hPa\nWilgotność: {humidity}%\nOpis: {description}";
                        WeatherIcon.Source = new BitmapImage(new Uri($"http://openweathermap.org/img/w/{icon}.png"));
                    }
                    catch (HttpRequestException ex)
                    {
                        MessageBox.Show($"Wystąpił błąd podczas pobierania danych: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd pobierania danych pogodowych: " + ex.Message);
            }
        }

    }
}
