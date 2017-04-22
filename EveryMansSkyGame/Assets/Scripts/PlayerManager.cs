using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{

    public GameObject PlayerCreationPanel;
    public GameObject PlanetCreationPanel;
    public InputField PlayerUsernameField;

    public Player Player;
    public float PlayerBaseSpeed;
    public float PlayerSprintSpeed;

    public float PlayerRotateSpeedH;
    public float PlayerRotateSpeedV;

    private float _yaw;
    private float _pitch;

	// Use this for initialization
	void Start ()
	{
        PlayerCreationPanel.SetActive(false);
        PlanetCreationPanel.SetActive(false);
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

            PlayerCreationPanel.SetActive(false);
            Cursor.visible = false;
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
            PlayerCreationPanel.SetActive(false);
            Cursor.visible = false;
        }

    }

    private Player JsonToPlayer(JSONObject json)
    {
        if (json["id"] == null)
        {
            return null;
        }

        Player player = new Player
        {
            Id = json["id"].ToString(),
            Username = json["username"].ToString(),
            PlanetsCreated = new List<string>()
        };

        if (json["planetsCreated"].list != null)
        {
            foreach (JSONObject planet in json["planetsCreated"].list)
            {
                player.PlanetsCreated.Add(planet.ToString());
            }
        }

        if (json["planetsDiscovered"].list != null)
        {
            player.PlanetsDiscovered = new List<string>();
            foreach (JSONObject planet in json["planetsDiscovered"].list)
            {
                player.PlanetsDiscovered.Add(planet.ToString());
            }
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
	        if (Input.GetKeyDown(KeyCode.Space))
	        {
	            TogglePlanetCreationMenu();
	        }
	        if (!PlanetCreationPanel.activeSelf)
	        {
	            float speed;
	            if (Input.GetKey(KeyCode.LeftShift))
	            {
	                speed = PlayerSprintSpeed;
	            }
	            else
	            {
	                speed = PlayerBaseSpeed;
	            }

	            if (Input.GetKey(KeyCode.W))
	            {
	                gameObject.transform.Translate(Vector3.forward * speed);
	            }
	            if (Input.GetKey(KeyCode.S))
	            {
	                gameObject.transform.Translate(Vector3.back * speed);
	            }
	            if (Input.GetKey(KeyCode.D))
	            {
	                gameObject.transform.Translate(Vector3.right * speed);
	            }
	            if (Input.GetKey(KeyCode.A))
	            {
	                gameObject.transform.Translate(Vector3.left * speed);
	            }
	            if (Input.GetKey(KeyCode.Z))
	            {
	                gameObject.transform.Translate(Vector3.up * speed);
	            }
	            if (Input.GetKey(KeyCode.X))
	            {
	                gameObject.transform.Translate(Vector3.down * speed);
	            }
	            if (Input.GetMouseButtonDown(0))
	            {
	                AttemptDiscoverPlanet();
	            }

	            _yaw += PlayerRotateSpeedH * Input.GetAxis("Mouse X");
	            _pitch -= PlayerRotateSpeedV * Input.GetAxis("Mouse Y");

	            transform.eulerAngles = new Vector3(_pitch, _yaw, 0.0f);
	        }
	    }
	}

    private void AttemptDiscoverPlanet()
    {
        
    }

    private void TogglePlanetCreationMenu()
    {
        if (PlanetCreationPanel.activeSelf)
        {
            PlanetCreationPanel.SetActive(false);
            Cursor.visible = false;
        }
        else
        {
            PlanetCreationPanel.SetActive(true);
            Cursor.visible = true;
        }
    }
}
