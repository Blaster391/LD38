using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public GameObject WarningMessage;
    public GameObject PlayerCreationPanel;
    public GameObject PlayerCreationSubPanel;
    public GameObject PlanetCreationPanel;
    public GameObject Crosshair;
    public GameObject ExitMenu;
    public InputField PlayerUsernameField;
    public InputField PlanetNameField;
    public Player Player;
    public float PlayerBaseSpeed;
    public float PlayerSprintSpeed;

    public float PlayerRotateSpeedH;
    public float PlayerRotateSpeedV;

    private float _yaw;
    private float _pitch;

    public Text ErrorText;

	// Use this for initialization
	void Start ()
	{
        Cursor.lockState = CursorLockMode.Confined;
        PlayerCreationPanel.SetActive(false);
        PlanetCreationPanel.SetActive(false);
        Crosshair.SetActive(false);
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
        var name = PlayerUsernameField.text.Trim();
        name = name.Replace(" ", "-");
        name = name.Replace("\"", "");

        if (name == string.Empty)
        {
            ErrorText.text = "Please enter a name";
            return;
        }

        if (name.Length > 100)
        {
            ErrorText.text = "Name too long";
            return;
        }

        //Validate fucking swearwords????
        StartCoroutine(CreatePlayer(name));
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

            using (System.IO.StreamWriter file = new System.IO.StreamWriter("user.usr", false))
            {
                file.WriteLine(id);
            }

            PlayerCreationPanel.SetActive(false);
            Crosshair.SetActive(true);
            Cursor.visible = false;
        }
        else
        {
            ErrorText.text = www.text;
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
            try
            {
                Player = JsonToPlayer(new JSONObject(www.text));
            }
            catch{}

            if (Player != null)
            {
                PlayerCreationPanel.SetActive(false);
                Crosshair.SetActive(true);
                Cursor.visible = false;
            }
            else
            {
                ShowPlayerCreationPanel();
            }
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
            Id = json["id"].ToString().Replace("\"", ""),
            Username = json["username"].ToString().Replace("\"", ""),
            PlanetsCreated = new List<string>()
        };

        if (json["planetsCreated"].list != null)
        {
            foreach (JSONObject planet in json["planetsCreated"].list)
            {
                player.PlanetsCreated.Add(planet.ToString().Replace("\"", ""));
            }
        }

        if (json["planetsDiscovered"].list != null)
        {
            player.PlanetsDiscovered = new List<string>();
            foreach (JSONObject planet in json["planetsDiscovered"].list)
            {
                player.PlanetsDiscovered.Add(planet.ToString().Replace("\"", ""));
            }
        }

        return player;
    }

    private void ShowPlayerCreationPanel()
    {
        if (File.Exists("user.usr"))
        {
            WarningMessage.SetActive(true);
            PlayerCreationSubPanel.SetActive(false);
        }
        else
        {
            WarningMessage.SetActive(false);
            PlayerCreationSubPanel.SetActive(true);
        }
        PlayerCreationPanel.SetActive(true);
        Crosshair.SetActive(false);
    }

    public void ShowCreationSubPanel()
    {
        PlayerCreationSubPanel.SetActive(true);
    }

    public void RetryLoadPlayerFromFile()
    {
        StartCoroutine(LoadUserFromFile());
    }

    // Update is called once per frame
    void Update ()
    {
        if (ExitMenu.activeSelf)
            return;


	    if (Player != null)
	    {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Cursor.visible = true;
            }
            if (Input.GetKeyUp(KeyCode.Tab))
            {
                Cursor.visible = false;
            }
            if (Input.GetKey(KeyCode.Tab))
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Space))
	        {
                if (!PlanetNameField.isFocused)
                {
                    TogglePlanetCreationMenu();
                }
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

    public float discoverRange;
    private void AttemptDiscoverPlanet()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, gameObject.transform.forward);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.distance < discoverRange)
            {
                if (hit.collider.gameObject.GetComponent<PlanetHolder>() != null)
                {

                    hit.collider.gameObject.GetComponent<PlanetHolder>().DiscoverPlanet(Player);
                }
            }
        }
    }

    private void TogglePlanetCreationMenu()
    {
        if (PlanetCreationPanel.activeSelf)
        {
            PlanetCreationPanel.SetActive(false);
            Cursor.visible = false;
            Crosshair.SetActive(true);
        }
        else
        {
            PlanetCreationPanel.SetActive(true);
            Cursor.visible = true;
            Crosshair.SetActive(false);
        }
    }
}
