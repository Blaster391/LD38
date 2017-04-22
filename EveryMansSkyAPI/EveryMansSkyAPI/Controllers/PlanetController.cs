using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EveryMansSkyAPI.Models;
using EveryMansSkyAPI.Raven;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EveryMansSkyAPI.Controllers
{
    [Route("api/[controller]")]
    public class PlanetController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<Planet> Get(int page)
        {
            return null;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(string id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]Planet value)
        {
            //VALIDATE
            var creator = RavenContext.LoadById<Player>(value.CreateByUser);
            if (creator == null)
                return;

            RavenContext.Save(value);
            if (creator.PlanetsCreated.All(x => x != value.Id))
            {
                creator.PlanetsCreated.Add(value.Id);
                RavenContext.Save(creator);
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
