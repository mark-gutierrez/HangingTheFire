using CloudNative.Common.Helpers;
using Dapper.Contrib.Extensions;
using HangingTheFire.Controllers;
using System.Diagnostics;
using System.Text;
using Xunit;
using AutoMapper;


namespace TestProject1
{
    public class UnitTest1
    {
        [Fact]
        public async Task TestGetWeatherData()
        {
            WeatherModel weather = await WeatherController.GetWeatherData();
            Assert.IsType<WeatherModel>(weather);
        }
        
        [Fact]
        public async Task TestCreateTableWeatherData()
        {
            var sb = new StringBuilder();
            sb.AppendLine(DatabaseTableScripter.CreateTableScript<Current>(true));

            Debug.WriteLine(sb.ToString());
            Console.WriteLine(sb.ToString());
        }
        
        [Fact]
        public async Task TestInsertIntoWeatherTable()
        {
            WeatherModel weather = await WeatherController.GetWeatherData();
            Console.WriteLine(weather);
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<WeatherModel, Current>();
            });
            var mapper = new Mapper(config);
            try
            {
                using (var db = DALHelper.GetConnection(WeatherController.GetConnectionString()))
                {
                    var mapped = mapper.Map<Current>(weather.Current);
                    await db.InsertAsync(mapped);
                }
                //await db.InsertAsync<WeatherModel>(weather.Current);
            }
            catch (Exception e)
            {
                throw;
            }
        }
        [Fact]
        public async Task TestWeatherPipeline()
        {
            await WeatherController.WeatherPipeline();
        }
    }
}