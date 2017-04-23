using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EveryMansSkyAPI.Models;
using EveryMansSkyAPI.Raven;
using EveryMansSkyAPI.Raven.Indexes;
using Microsoft.AspNetCore.Mvc;
using Raven.Client;

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
            var name = username.Trim();
            if (name == string.Empty)
            {
                return "Please enter a name";
            }

            if (name.Length > 100)
                return "Name too long";

            var swears = Swears.GetSwears();
            foreach (var swear in swears)
            {
                if (name.ToLower().Contains(swear))
                {
                    return "Invalid name";
                }
            }
            
            using (var session = RavenContext.Store.OpenSession())
            {
                var check = session.Query<Player>().FirstOrDefault(x => x.Username == name);
                if (check != null)
                {
                    return "Username taken";
                }
            }

            var id = "PLAYER." + Guid.NewGuid();
            Player player = new Player
            {
                Id = id,
                Username = name
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
                if (planet.CreatedByUserId != player.Id)
                {
                    if (player.PlanetsDiscovered == null)
                    {
                        player.PlanetsDiscovered = new List<string>();
                    }

                    if (!player.PlanetsDiscovered.Contains(planet.Id))
                    {
                        player.PlanetsDiscovered.Add(planet.Id);
                        RavenContext.Save(player);
                    }
                }
            }

            return player.PlanetsDiscovered.Count;
        }

        [HttpGet("score/discovered")]
        public List<PlayerScore> GetHighScoresPlanetsDiscovered(int page = 0)
        {
            using (var session = RavenContext.Store.OpenSession())
            {
                    var topPlayers = session.Query<PlayerScore, ScoreMap>()
                        .OrderByDescending(x => x.PlanetsDiscovered)
                        .As<Player>()
                        .Take(100)
                        .ToList();

                List<PlayerScore> topScores = new List<PlayerScore>();
                foreach (var player in topPlayers)
                {
                    topScores.Add(new PlayerScore
                    {
                        PlayerUsername = player.Username,
                        PlanetsCreated = player.PlanetsCreated?.Count ?? 0,
                        PlanetsDiscovered = player.PlanetsDiscovered?.Count ?? 0
                    });
                   
                }
                return topScores;
            }
        }

        [HttpGet("score/created")]
        public List<PlayerScore> GetHighScoresPlanetsCreated(int page = 0)
        {
            using (var session = RavenContext.Store.OpenSession())
            {
                var topPlayers = session.Query<PlayerScore, ScoreMap>()
                    .OrderByDescending(x => x.PlanetsCreated)
                    .As<Player>()
                    .Take(100)
                    .ToList();

                List<PlayerScore> topScores = new List<PlayerScore>();
                foreach (var player in topPlayers)
                {
                    topScores.Add(new PlayerScore
                    {
                        PlayerUsername = player.Username,
                        PlanetsCreated = player.PlanetsCreated?.Count ?? 0,
                        PlanetsDiscovered = player.PlanetsDiscovered?.Count ?? 0
                    });

                }
                return topScores;
            }
        }
    }
}
