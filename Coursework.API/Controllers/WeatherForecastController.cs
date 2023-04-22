using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Coursework.API.Controllers
{
    //[ApiController]
    //[Route("[controller]")] WeatherForecast
    public class TestControllers : ControllerBase
    {
        //private static readonly string[] Summaries = new[]
        //{
        //"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        //};

        private readonly ILogger<TestControllers> _logger;

        public TestControllers(ILogger<TestControllers> logger)
        {
            _logger = logger;
        }

        //[HttpGet(Name = "GetWeatherForecast")]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = Random.Shared.Next(-20, 55),
        //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}
        //[HttpGet(Name = "GetWeatherForecast")]
        [HttpGet]
        [Route("/api/themandalorian")]
        public List<List<string>> Mando()
        {
            List<List<string>> results = new List<List<string>>()
           {
               new List<string> { "the hiress", "the jedi", "the rescue" },
               new List<string> { "mines of mandalore", "the darksaber", "the return" }

           };
            return results;
        }
    }
}