using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{

    public GameObject PlayerCreationPanel;
    public InputField PlayerUsernameField;

    public Player Player;
	// Use this for initialization
	void Start ()
	{
	    if (File.Exists("user.usr"))
	    {
            StartCoroutine(LoadUserFromFile());
	    }

	    if (Player == null)
	    {
	        ShowPlayerCreationPanel();
	    }
	}

    public void CreatePlayerFromPanel()
    {
        if (PlayerUsernameField.text.Trim() == string.Empty)
        {
            return;
        }
        //Validate fucking swearwords????
        StartCoroutine(CreatePlayer(PlayerUsernameField.text));
    }

    public IEnumerator CreatePlayer(string username)
    {
        var www = new WWW(WebApiAccess.ApiUrl + "/player/create?username="+username);

        yield return www;

        //Next level validation fam
        if (www.text.StartsWith("PLAYER."))
        {
            var id = www.text;
            Player = new Player
            {
                Id = id,
                Username = username,
                PlanetsCreated = new List<string>(),
                PlanetsDiscovered = new List<string>()
            };

            using (System.IO.StreamWriter file = new System.IO.StreamWriter("user.usr",false))
            {
                file.WriteLine(id);
            }
        }
    }

    private IEnumerator LoadUserFromFile()
    {
        System.IO.StreamReader file = new System.IO.StreamReader("user.usr");
        string id;
        if ((id = file.ReadLine()) != null)
        {
            var www = new WWW(WebApiAccess.ApiUrl + "/player/" + id);
            yield return www;

            Player = JsonToPlayer(new JSONObject(www.text));
        }

    }

    private Player JsonToPlayer(JSONObject json)
    {
        Player player = new Player();
        player.Id = json["id"].ToString();
        player.Username = json["username"].ToString();

        player.PlanetsCreated = new List<string>();
        foreach (JSONObject planet in json["planetsCreated"].list)
        {
            player.PlanetsCreated.Add(planet.ToString());
        }

        player.PlanetsDiscovered = new List<string>();
        foreach (JSONObject planet in json["planetsDiscovered"].list)
        {
            player.PlanetsDiscovered.Add(planet.ToString());
        }

        return player;
    }

    private void ShowPlayerCreationPanel()
    {
        if (File.Exists("user.usr"))
        {
            //TODO SHOW WARNING MESSAGE 
        }
        PlayerCreationPanel.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {

	    if (Player != null)
	    {
	        //Controls and stuff yo
	    }
	}
}
