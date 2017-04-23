using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreHandler : MonoBehaviour
{
    public GameObject ScorePanel;
    public ScorePanelScript DiscoveredPanel;
    public ScorePanelScript CreatedPanel;


    // Use this for initialization
    void Start () {
        ScorePanel.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Tab))
	    {
            ScorePanel.SetActive(true);
	        StartCoroutine(ScorePanelUpdate());
	    }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            ScorePanel.SetActive(false);
        }
    }

    IEnumerator ScorePanelUpdate()
    {
        while (ScorePanel.activeSelf)
        {
            var wwwDiscovered = new WWW(WebApiAccess.ApiUrl + "/player/score/discovered");
            var wwwCreated = new WWW(WebApiAccess.ApiUrl + "/player/score/created");
            yield return wwwDiscovered;
            yield return wwwCreated;

            var discoveredScores = JsonToScore(new JSONObject(wwwDiscovered.text));
            var createdScores = JsonToScore(new JSONObject(wwwCreated.text));

            DiscoveredPanel.PopulatePanel(discoveredScores);
            CreatedPanel.PopulatePanel(createdScores);

            yield return new WaitForSeconds(5);
        }
    }

    private List<PlayerScore> JsonToScore(JSONObject json)
    {

        List<PlayerScore> scores = new List<PlayerScore>();
        if (json.list != null)
        {
            foreach (var scoreJson in json.list)
            {
                PlayerScore score = new PlayerScore
                {
                    PlayerUsername = scoreJson["playerUsername"].ToString().Replace("\"", ""),
                    PlanetsCreated = int.Parse(scoreJson["planetsCreated"].ToString().Replace("\"", "")),
                    PlanetsDiscovered = int.Parse(scoreJson["planetsDiscovered"].ToString().Replace("\"", ""))
                };
                scores.Add(score);
            }
        }

        return scores;
    }

}
