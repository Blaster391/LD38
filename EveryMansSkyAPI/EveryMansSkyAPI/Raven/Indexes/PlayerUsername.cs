using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EveryMansSkyAPI.Models;
using Raven.Client.Indexes;

namespace EveryMansSkyAPI.Raven.Indexes
{
    public class PlayerUsername : AbstractIndexCreationTask<Player>
    {
        public class Result
        {
            public string Username { get; set; }
        }

        public PlayerUsername()
        {
            Map = players => from player in players
                            select new
                            {
                                player.Username
                            };
        }
    }
}
