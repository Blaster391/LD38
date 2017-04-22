using System;
using UnityEngine;
using System.Collections;

public class Planet {

    public string Id { get; set; }
    public string Name { get; set; }
    public string CreateByUserId { get; set; }
    public string CreateByUsername { get; set; }
    public PlanetType Type { get; set; }
    public float Size { get; set; }
    public float ColourRed { get; set; }
    public float ColourGreen { get; set; }
    public float ColourBlue { get; set; }
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float PositionZ { get; set; }
    public float RotationX { get; set; }
    public float RotationY { get; set; }
    public float RotationZ { get; set; }
    public DateTime LastModified { get; set; }
    public int Upvotes { get; set; }
}
