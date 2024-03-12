
using AutoMapper;
using CloudNative.Common.Helpers;
using Dapper.Contrib.Extensions;
using HangingTheFire.Jobs;
using Newtonsoft.Json;
using System.Drawing.Drawing2D;

namespace HangingTheFire.Controllers
{
    public class WeatherController
    {
        private readonly ILogger _logger;
        public WeatherController(ILogger<WeatherController> logger) => _logger = logger;

        public static string? GetConnectionString()
        {
            var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot _configuration = builder.Build();

            return _configuration.GetConnectionString("DbConnection");
        }

        public static async Task<WeatherModel?> GetWeatherData(double latitude = -33.865143, double longitude = 151.209900)
        {
            string WEATHER_API = "235928c86c4d4073b6632829242802";
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync($"https://api.weatherapi.com/v1/current.json?key={WEATHER_API}&q={latitude},{longitude}");
                var json = await response.Content.ReadAsStringAsync();
                var weather = JsonConvert.DeserializeObject<WeatherModel>(json);

                return weather;
            }
        }

        public static async Task InsertIntoDatabase(Current weather)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<WeatherModel, Current>();
            });
            var mapper = new Mapper(config);
            string? connectionString = GetConnectionString();
            try
            {
                using (var db = DALHelper.GetConnection(connectionString))
                {
                    var mapped = mapper.Map<Current>(weather);
                    await db.InsertAsync(mapped);
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task WeatherPipeline()
        {
            _logger.LogInformation($"{DateTime.Now:yyyy-MM-dd hh:mm:ss tt} Started Weather Pipeline");
            WeatherModel? weather = await GetWeatherData();
            await InsertIntoDatabase(weather.Current);
            _logger.LogInformation($"{DateTime.Now:yyyy-MM-dd hh:mm:ss tt} Finished Weather Pipeline");
        }
    }
}
