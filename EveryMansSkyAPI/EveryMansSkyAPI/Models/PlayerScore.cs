using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EveryMansSkyAPI.Models
{
    public class PlayerScore
    { 
        public string PlayerUsername { get; set; }
        public int PlanetsDiscovered { get; set; }
        public int PlanetsCreated { get; set; }
    }
}
