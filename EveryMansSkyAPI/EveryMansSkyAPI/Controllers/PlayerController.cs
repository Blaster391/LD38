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
    public class PlayerController : Controller
    {
        // GET api/values/5
        [HttpGet("{id}")]
        public Player Get(string id)
        {
            return RavenContext.LoadById<Player>(id);
        }

        [HttpGet("create")]
        public string Create(string username)
        {
            //VALIDATE
            using (var session = RavenContext.Store.OpenSession())
            {
                var check = session.Query<Player>().FirstOrDefault(x => x.Username == username);
                if (check != null)
                {
                    return "FAILED";
                }
            }

            var id = "PLAYER." + Guid.NewGuid();
            Player player = new Player
            {
                Id = id,
                Username = username
            };
            RavenContext.Save(player);

            return id;
        }

        [HttpGet("discover")]
        public int Discover(string playerId, string planetId)
        {
            var player = RavenContext.LoadById<Player>(playerId);
            if (player == null)
                return 0; //TODO return -1?

            var planet = RavenContext.LoadById<Planet>(planetId);
            if (planet != null)
            {
                if (planet.CreateByUserId != player.Id)
                {
                    if (!player.PlanetsDiscovered.Contains(planet.Id))
                    {
                        player.PlanetsDiscovered.Add(planet.Id);
                        RavenContext.Save(player);
                    }
                }
            }

            return player.PlanetsDiscovered.Count;
        }
    }
}
