using UnityEngine;

public class PlanetHolder : MonoBehaviour
{

    public Planet Planet;
    public TextMesh PlanetText;

    public void SetName(string planetName)
    {
        PlanetText.text = planetName;
    }
}
