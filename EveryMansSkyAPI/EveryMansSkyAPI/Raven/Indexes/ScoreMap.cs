using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EveryMansSkyAPI.Models;
using Raven.Client.Indexes;

namespace EveryMansSkyAPI.Raven.Indexes
{
    public class ScoreMap : AbstractIndexCreationTask<Player, ScoreMap.Result>
    {
        public class Result
        {
            public string PlayerId { get; set; }
            public string Username { get; set; }
            public int PlanetsDiscovered { get; set; }
            public int PlanetsCreated { get; set; }

        }

        public ScoreMap()
        {
            Map = players => from player in players
                              select new
                              {
                                  PlayerId = player.Id,
                                  Username = player.Username,
                                  PlanetsDiscovered = player.PlanetsDiscovered.Count,
                                  PlanetsCreated = player.PlanetsCreated.Count
                              };

        }
    }
}
