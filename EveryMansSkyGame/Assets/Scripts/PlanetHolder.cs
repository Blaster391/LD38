using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetHolder : MonoBehaviour
{

    public Planet Planet;
    public TextMesh PlanetText;

    public Material YellowText;
    public Material GreenText;
    public Material NormalText;
    public void SetName(string planetName)
    {
        PlanetText.text = planetName;
    }

    public void SetTextColour(Player player)
    {
        if (player.PlanetsCreated != null && player.PlanetsCreated.Contains(Planet.Id))
        {
            PlanetText.GetComponent<MeshRenderer>().material = YellowText;
        }
        else if (player.PlanetsDiscovered != null && player.PlanetsDiscovered.Contains(Planet.Id))
        {
            PlanetText.GetComponent<MeshRenderer>().material = GreenText;
        }
        else
        {
            PlanetText.GetComponent<MeshRenderer>().material = NormalText;
        }
    }

    public void DiscoverPlanet(Player player)
    {
        if (player.PlanetsCreated != null && !player.PlanetsCreated.Contains(Planet.Id))
        {
            if (player.PlanetsDiscovered == null)
            {
                player.PlanetsDiscovered = new List<string>();
            }
            if (!player.PlanetsDiscovered.Contains(Planet.Id))
            {
                player.PlanetsDiscovered.Add(Planet.Id);
                StartCoroutine(DiscoverPlanet(player.Id, Planet.Id));
            }
        }

        SetTextColour(player);
    }

    public IEnumerator DiscoverPlanet(string playerId, string planetId)
    {
        var www = new WWW(WebApiAccess.ApiUrl + "/player/discover?playerId=" + playerId + "&planetId=" + planetId);
        yield return www;
    }
}
