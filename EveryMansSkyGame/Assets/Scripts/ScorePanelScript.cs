using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ScorePanelScript : MonoBehaviour
{
    public PlayerManager PlayerManager;
    public GameObject ScorecardPrefab;
    public GameObject ContentPanel;
    private List<GameObject> _scorePanels = new List<GameObject>();

    void Start()
    {
        PlayerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
    }
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
            var rectTransform = scorecard.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, -25 + (-50 * rank));
            scorecard.transform.localScale = new Vector3(1,1,1);

            bool highlight = PlayerManager.Player.Username == score.PlayerUsername;

            scorecard.GetComponent<Scorecard>().Initialize(score, rank, highlight);
            _scorePanels.Add(scorecard);
            rank++;
        }
    }
}
