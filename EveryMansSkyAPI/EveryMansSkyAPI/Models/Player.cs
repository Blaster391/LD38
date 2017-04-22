using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EveryMansSkyAPI.Models
{
    public class Player
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public List<string> PlanetsCreated { get; set; }
        public List<string> PlanetsDiscovered { get; set; }

    }
}
