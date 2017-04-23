using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Scorecard : MonoBehaviour
{
    public Text Rank;
    public Text Username;
    public Text DiscoveredPlanets;
    public Text CreatedPlanets;
    public Image Image;
    public void Initialize(PlayerScore score, int rank, bool highlight)
    {
        Rank.text = rank.ToString();
        Username.text = score.PlayerUsername;
        DiscoveredPlanets.text = score.PlanetsDiscovered.ToString();
        CreatedPlanets.text = score.PlanetsCreated.ToString();

        if (highlight)
        {
            Username.color = Color.yellow;
        }
    }
}
