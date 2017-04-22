using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player  {
    public string Id { get; set; }
    public string Username { get; set; }
    public List<string> PlanetsCreated { get; set; }
    public List<string> PlanetsDiscovered { get; set; }
}
