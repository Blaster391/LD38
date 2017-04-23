using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerScoreScript : MonoBehaviour
{
    public GameObject PlayerScorePanel;
    public PlayerManager PlayerManager;

    public Text Username;
    public Text PlanetsDiscovered;
    public Text PlanetsCreated;
	// Use this for initialization
	void Start () {
        PlayerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        PlayerScorePanel.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
	{
	    if (PlayerManager.Player != null)
	    {
            PlayerScorePanel.SetActive(true);
            Username.text = PlayerManager.Player.Username;
	        if (PlayerManager.Player.PlanetsDiscovered != null)
	        {
                PlanetsDiscovered.text = PlayerManager.Player.PlanetsDiscovered.Count.ToString();
            }
	        if (PlayerManager.Player.PlanetsCreated != null)
	        {
                PlanetsCreated.text = PlayerManager.Player.PlanetsCreated.Count.ToString();
            }
        }
	    else
	    {
            PlayerScorePanel.SetActive(false);
	    }
	}
}
