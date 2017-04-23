using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EveryMansSkyAPI.Models;
using Raven.Client.Indexes;

namespace EveryMansSkyAPI.Raven.Indexes
{
    public class ScoreMap : AbstractIndexCreationTask<Player, PlayerScore>
    {

        public ScoreMap()
        {
            Map = players => from player in players
                              select new
                              {
                                  PlayerUsername = player.Username,
                                  PlanetsDiscovered = (player.PlanetsDiscovered == null) ? 0 : player.PlanetsDiscovered.Count,
                                  PlanetsCreated = (player.PlanetsCreated == null) ? 0 : player.PlanetsCreated.Count
                              };

        }
    }
}
