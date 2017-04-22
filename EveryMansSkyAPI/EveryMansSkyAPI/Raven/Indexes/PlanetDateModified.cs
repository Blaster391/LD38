using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EveryMansSkyAPI.Models;
using Raven.Client.Indexes;

namespace EveryMansSkyAPI.Raven.Indexes
{
    public class PlanetDateModified : AbstractIndexCreationTask<Planet>
    {
        public class Result
        {
            public DateTime LastModified { get; set; }
        }

        public PlanetDateModified()
        {
            Map = planets => from planet in planets
                             select new
                             {
                                 planet.LastModified
                             };
        }
    }
}
