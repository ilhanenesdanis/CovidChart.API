using CovidChart.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CovidChart.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CovidController : ControllerBase
    {
        private readonly CovidService _service;

        public CovidController(CovidService service)
        {
            _service = service;
        }
        [HttpPost]
        public async Task<IActionResult> SaveCovid(Covid covid)
        {
            await _service.SaveCovid(covid);
            //IQueryable<Covid> covidList = _service.GetAllList();
            return Ok(_service.GetCovidList());
        }
        [HttpGet]
        public  IActionResult GetAllCovid()
        {
            Random rnd = new Random();

            Enumerable.Range(1, 10).ToList().ForEach(async x =>
            {
                foreach(City item in Enum.GetValues(typeof(City)))
                {
                    var newCovid = new Covid { City = item, Count = rnd.Next(100, 10000), CovidDate = DateTime.Now.AddDays(x) };
                    _service.SaveCovid(newCovid).Wait();
                    System.Threading.Thread.Sleep(100);
                }

            });
            return Ok("Covid19 Dataları Veritabanına Kaydedildi");
        }
    }
}
