using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class PlanetManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Player = GameObject.Find("Player").GetComponent<PlayerManager>();
	    StartCoroutine(HeartbeatLoop());

	}

    IEnumerator HeartbeatLoop()
    {
        while (true)
        {
            while (Player.Player == null)
            {
                yield return new WaitForEndOfFrame();
            }
            try
            {
                StartCoroutine(PlanetLoaderLoop());
                StartCoroutine(PlanetRecentLoadingLoop());
            }
            catch{}
            yield return new WaitForSeconds(400);
        }
    }


	// Update is called once per frame
    private GameObject _previewPlanet;
	void Update () {
        if (_previewPlanet != null)
        {
            Destroy(_previewPlanet);
            _previewPlanet = null;
        }

        if (PlanetCreationPanel.activeSelf)
	    {
            Cursor.visible = true;
            var color = new Color
            {
                r = PlanetColourRedSlider.value,
                g = PlanetColourGreenSlider.value,
                b = PlanetColourBlueSlider.value,
                a = 1
            };

            var previewImage = ColourPickerPreviewPanel.GetComponent<Image>();
            previewImage.color = color;


            if (PreviewToggle.isOn)
	        {


	            var previewPlanet = GeneratePlanetObject();
	            previewPlanet.Id = "PREVIEW";
                _previewPlanet = SpawnPlanet(previewPlanet, true);

                var planetRenderer = _previewPlanet.GetComponent<Renderer>();
	            var collider = _previewPlanet.GetComponent<Collider>();
	            collider.enabled = false;

                var previewColor = new Color
                {
                    r = PlanetColourRedSlider.value,
                    g = PlanetColourGreenSlider.value,
                    b = PlanetColourBlueSlider.value,
                    a = 0.5f
                };

                var position = Player.transform.position + Player.transform.forward * WorldSpawnDistance;
                if (Physics.CheckSphere(position, previewPlanet.Size))
	            {
                    previewColor = new Color
                    {
                        r = 1,
                        g = 0,
                        b = 0,
                        a = 0.3f
                    };
                }

                planetRenderer.material.color = previewColor;
            }
        }
    }
    public GameObject PlanetBasicPrefab;
    public GameObject PlanetRingedPrefab;

    public GameObject PlanetCreationPanel;
    public GameObject ColourPickerPreviewPanel;
    public InputField PlanetNameField;
    public GameObject Crosshair;
    public Toggle PlanetRingToggle;
    public Toggle PreviewToggle;
    public Slider PlanetColourRedSlider;
    public Slider PlanetColourBlueSlider;
    public Slider PlanetColourGreenSlider;

    public Slider PlanetRotationXSlider;
    public Slider PlanetRotationYSlider;
    public Slider PlanetRotationZSlider;
    public Slider PlanetSizeSlider;

    public PlayerManager Player;
    public float WorldSizeMod = 10;
    public float WorldSpawnDistance = 10;

    private Dictionary<string,PlanetHolder> PlanetDictionary = new Dictionary<string, PlanetHolder>();
    public void AttemptCreatePlanet()
    {
        var planetName = PlanetNameField.text;
        if (planetName.Trim() == string.Empty)
            return;

        var swears = new List<string>
            {
                "fuck",
                "shit",
                "nigga",
                "nigger",
                "cunt",
                "gay",
                "faggot",
                "huskar you okay my buddie?",
                "cock",
                "wanker",
                "slut",
                "niggg",
                "dick"
            };
        foreach (var swear in swears)
        {
            if (planetName.ToLower().Contains(swear))
                return;
        }
        

        var planet = GeneratePlanetObject();
        var position = Player.transform.position + Player.transform.forward * WorldSpawnDistance;

        if (_previewPlanet != null)
        {
            Destroy(_previewPlanet);
            _previewPlanet = null;
        }

        if (Physics.CheckSphere(position, planet.Size))
        {
            return;
        }

        SpawnPlanet(planet, false);
        SavePlanet(planet);
        if (Player.Player.PlanetsCreated == null)
        {
            Player.Player.PlanetsCreated = new List<string>();
        }
        if (!Player.Player.PlanetsCreated.Contains(planet.Id))
        {
            Player.Player.PlanetsCreated.Add(planet.Id);
        }
        PlanetCreationPanel.SetActive(false);
        Cursor.visible = false;
        Crosshair.SetActive(true);
    }

    private Planet GeneratePlanetObject()
    {
        var position = Player.transform.position + Player.transform.forward * WorldSpawnDistance;
        var planetName = PlanetNameField.text;

        var color = new Color
        {
            r = PlanetColourRedSlider.value,
            g = PlanetColourGreenSlider.value,
            b = PlanetColourBlueSlider.value
        };

        var size = PlanetSizeSlider.value * WorldSizeMod;
        if (size < 0.000001)
        {
            return null;
        }

        var rotation = new Vector3(PlanetRotationXSlider.value * 360, PlanetRotationYSlider.value * 360, PlanetRotationZSlider.value * 360);

        var planetType = PlanetType.Basic;
        if (PlanetRingToggle.isOn)
            planetType = PlanetType.Ringed;

        var planet = new Planet
        {
            Id = "PLANET." + Guid.NewGuid(),
            Name = planetName,
            PositionX = position.x,
            PositionY = position.y,
            PositionZ = position.z,
            RotationX = rotation.x,
            RotationY = rotation.y,
            RotationZ = rotation.z,
            Size = size,
            ColourRed = color.r,
            ColourGreen = color.g,
            ColourBlue = color.b,
            Type = planetType,
            CreatedByUserId = Player.Player.Id,
            CreatedByUsername = Player.Player.Username
        };
        return planet;
    }

    public GameObject SpawnPlanet(Planet planet, bool preview)
    {
        if (PlanetDictionary.ContainsKey(planet.Id))
        {
            GameObject.Destroy(PlanetDictionary[planet.Id].gameObject);
            PlanetDictionary.Remove(planet.Id);
        }

        GameObject prefab;
        switch (planet.Type)
        {
            case PlanetType.Basic:
                prefab = PlanetBasicPrefab;
                break;
            default:
                prefab = PlanetRingedPrefab;
                break;
        }

        var newPlanet = Instantiate(prefab);

        var position = new Vector3(planet.PositionX, planet.PositionY, planet.PositionZ);
        newPlanet.transform.position = position;
        newPlanet.transform.eulerAngles = new Vector3(planet.RotationX, planet.RotationY, planet.RotationZ);

        var color = new Color
        {
            r = planet.ColourRed,
            g = planet.ColourGreen,
            b = planet.ColourBlue,
            a = 1
        };

        newPlanet.transform.localScale = new Vector3(planet.Size, planet.Size, planet.Size);

        var planetRenderer = newPlanet.GetComponent<Renderer>();
        planetRenderer.material.color = color;

        if (planet.Type == PlanetType.Ringed)
        {
            var ring = newPlanet.transform.FindChild("ring");
            var ringRender =  ring.GetComponent<Renderer>();
            ringRender.material.color = color;
        }

        var planetHolder = newPlanet.GetComponent<PlanetHolder>();
        planetHolder.Planet = planet;
        planetHolder.SetName(planet.Name);
        planetHolder.SetTextColour(Player.Player);
        if (!preview)
        {
            PlanetDictionary.Add(planet.Id, planetHolder);
        }
        return newPlanet;
    }

    private void SavePlanet(Planet planet)
    {
        StartCoroutine(SavePlanetCoroutine(planet));
    }

    private IEnumerator SavePlanetCoroutine(Planet planet)
    {
        var headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");

        string json = "{";
        json += WebApiAccess.ToJsonElement("Id", planet.Id.Replace("\"", ""));
        json += ",";
        json += WebApiAccess.ToJsonElement("Name", planet.Name.Replace("\"", ""));
        json += ",";
        json += WebApiAccess.ToJsonElement("CreatedByUserId", planet.CreatedByUserId.Replace("\"", ""));
        json += ",";
        json += WebApiAccess.ToJsonElement("Size", planet.Size.ToString());
        json += ",";
        json += WebApiAccess.ToJsonElement("Type", planet.Type.ToString());
        json += ",";
        json += WebApiAccess.ToJsonElement("PositionX", planet.PositionX.ToString());
        json += ",";
        json += WebApiAccess.ToJsonElement("PositionY", planet.PositionY.ToString());
        json += ",";
        json += WebApiAccess.ToJsonElement("PositionZ", planet.PositionZ.ToString());
        json += ",";
        json += WebApiAccess.ToJsonElement("RotationX", planet.RotationX.ToString());
        json += ",";
        json += WebApiAccess.ToJsonElement("RotationY", planet.RotationY.ToString());
        json += ",";
        json += WebApiAccess.ToJsonElement("RotationZ", planet.RotationZ.ToString());
        json += ",";
        json += WebApiAccess.ToJsonElement("ColourRed", planet.ColourRed.ToString());
        json += ",";
        json += WebApiAccess.ToJsonElement("ColourGreen", planet.ColourGreen.ToString());
        json += ",";
        json += WebApiAccess.ToJsonElement("ColourBlue", planet.ColourBlue.ToString());
        json += "}";

        var www = new WWW(WebApiAccess.ApiUrl + "/Planet", Encoding.UTF8.GetBytes(json), headers);

        yield return www;
    }

    private IEnumerator PlanetLoaderLoop()
    {
        while (true)
        {
            yield return GetPlanets(DateTime.MinValue);
            yield return new WaitForSeconds(120);
        }
    }

    private DateTime lastFetchTime = DateTime.MinValue;
    private IEnumerator PlanetRecentLoadingLoop()
    {
        while (true)
        {
            yield return GetPlanets(lastFetchTime);
            lastFetchTime = DateTime.Now;
            yield return new WaitForSeconds(10);
        }
    }

    private IEnumerator GetPlanets(DateTime dateFrom)
    {
        int page = 0;
        bool finished = false;
        while (!finished)
        {
            var encodedDate = WWW.EscapeURL(dateFrom.ToString());
            var www = new WWW(WebApiAccess.ApiUrl + "/Planet?dateFrom=" + encodedDate + "&page=" + page);
            yield return www;

            var jsonList = new JSONObject(www.text);
            if (jsonList.list != null)
            {
                foreach (var jsonPlanet in jsonList)
                {
                    var planet = JsonToPlanet(jsonPlanet);
                    SpawnPlanet(planet, false);
                    yield return new WaitForEndOfFrame();
                }
            }

            if (jsonList.list == null || jsonList.list.Count == 0)
            {
                finished = true;
            }
            page++;
        }
    }

    private Planet JsonToPlanet(JSONObject json)
    {
        var elementObject = json.ToDictionary();
        Planet parsedPlanet = new Planet();
        parsedPlanet.Id = elementObject["id"].Replace("\"","");
        parsedPlanet.Name = elementObject["name"].Replace("\"", "");
        parsedPlanet.CreatedByUserId = elementObject["createdByUserId"];
        parsedPlanet.CreatedByUsername = elementObject["createdByUsername"];
        
        parsedPlanet.Size = float.Parse(elementObject["size"]);
        parsedPlanet.PositionX = float.Parse(elementObject["positionX"]);
        parsedPlanet.PositionY = float.Parse(elementObject["positionY"]);
        parsedPlanet.PositionZ = float.Parse(elementObject["positionZ"]);

        parsedPlanet.ColourRed = float.Parse(elementObject["colourRed"]);
        parsedPlanet.ColourGreen = float.Parse(elementObject["colourGreen"]);
        parsedPlanet.ColourBlue = float.Parse(elementObject["colourBlue"]);

        parsedPlanet.RotationX = float.Parse(elementObject["rotationX"]);
        parsedPlanet.RotationY = float.Parse(elementObject["rotationY"]);
        parsedPlanet.RotationZ = float.Parse(elementObject["rotationZ"]);

        parsedPlanet.Upvotes = int.Parse(elementObject["upvotes"]);

        var typeInt = int.Parse(elementObject["type"]);
        if (typeInt == 1)
        {
            parsedPlanet.Type = PlanetType.Ringed;
        }

        return parsedPlanet;
    }
}
