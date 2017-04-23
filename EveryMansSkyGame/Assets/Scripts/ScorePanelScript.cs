using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScorePanelScript : MonoBehaviour
{
    public GameObject ScorecardPrefab;
    public GameObject ContentPanel;
    private List<GameObject> _scorePanels = new List<GameObject>();
    public void PopulatePanel(List<PlayerScore> scores)
    {
        foreach (var panel in _scorePanels)
        {
            GameObject.Destroy(panel);
        }
        _scorePanels = new List<GameObject>();

        int rank = 1;
        foreach (var score in scores)
        {
            var scorecard = Instantiate(ScorecardPrefab);
            scorecard.transform.SetParent(ContentPanel.transform);
            scorecard.transform.localPosition = new Vector3(0, -40 * rank, 0);

            scorecard.GetComponent<Scorecard>().Initialize(score, rank);
            rank++;
        }
    }
}
