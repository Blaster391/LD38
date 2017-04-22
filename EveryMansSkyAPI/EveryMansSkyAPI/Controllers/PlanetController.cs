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
        public IEnumerable<Planet> Get(DateTime lastModified, int page = 0)
        {
            using (var session = RavenContext.Store.OpenSession())
            {
                return session.Query<Planet>().Take(128).Skip(128 * page).ToList();
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public Planet Get(string id)
        {
            return RavenContext.LoadById<Planet>(id);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]Planet value)
        {
            //VALIDATE
            var creator = RavenContext.LoadById<Player>(value.CreateByUserId);
            if (creator == null)
                return;
            value.CreateByUsername = creator.Username;

            RavenContext.Save(value);
            if (!creator.PlanetsCreated.Contains(value.Id))
            {
                creator.PlanetsCreated.Add(value.Id);
                RavenContext.Save(creator);
            }
        }
    }
}
