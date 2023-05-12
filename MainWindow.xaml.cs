using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace lawerna_weather
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public async Task Go(string city)
        {
            GetData weatherData = new GetData();
            this.Result.Document.Blocks.Clear();
            this.Result.AppendText(await weatherData.GetWeather(city));
        }

        private void Request_button_Click(object sender, RoutedEventArgs e)
        {
            this.Go(this.City.Text);
        }
    }

    class GetData
    {
        static HttpClient client = new HttpClient();
        public string result ="";

        public async Task<string> GetWeather(string city)
        {
            var result = "";
            HttpResponseMessage response = await client.GetAsync(@"https://api.openweathermap.org/data/2.5/weather?appid=ac0dffaa5d5efbc4bd7fb95f2126a810&q=" + city + @"&units=metric");
            if (response.IsSuccessStatusCode)
            {
                var s = await response.Content.ReadAsStreamAsync();

                using (var sr = new StreamReader(s))
                {
                    var contributorsAsJson = sr.ReadToEnd();
                    var contributors = JsonConvert.DeserializeObject<Product>(contributorsAsJson);
                    string windSpeed = contributors.wind.speed.ToString();
                    result += "city: " + contributors.name + " \r";
                    result += "temperature: " + contributors.main.temp + @" C" + " \r";
                    result += "description: " + contributors.weather[0].description + "\r";
                    result += "wind: " + windSpeed + @" m/s" + "\r";
                }
            }
            return result;
        }
    }

    internal class Product
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("main")]
        public MainObj main { get; set; }
        [JsonProperty("weather")]
        public Weather[] weather { get; set; }
        [JsonProperty("wind")]
        public Wind wind { get; set; }
    }

    internal class MainObj
    {
        [JsonProperty("temp")]
        public string temp { get; set; }
        [JsonProperty("feels_like")]
        public string feel_like { get; set; }
        [JsonProperty("humidity")]
        public string humidity { get; set; }
    }

    internal class Weather
    {
        [JsonProperty("description")]
        public string description { get; set; }
    }

    internal class Wind
    {
        [JsonProperty("speed")]
        public string speed { get; set; }
    }
}
