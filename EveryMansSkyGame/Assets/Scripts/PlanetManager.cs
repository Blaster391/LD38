using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class PlanetManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Player = GameObject.Find("Player").GetComponent<PlayerManager>();
	    StartCoroutine(PlanetLoaderLoop());
	    StartCoroutine(PlanetRecentLoadingLoop());
	}
	

	// Update is called once per frame
	void Update () {
	    if (PlanetCreationPanel.activeSelf)
	    {
            var color = new Color
            {
                r = PlanetColourRedSlider.value,
                g = PlanetColourGreenSlider.value,
                b = PlanetColourBlueSlider.value,
                a = 1
            };

            var previewImage = ColourPickerPreviewPanel.GetComponent<Image>();
            previewImage.color = color;
        }
    }
    public GameObject PlanetBasicPrefab;
    public GameObject PlanetRingedPrefab;

    public GameObject PlanetCreationPanel;
    public GameObject ColourPickerPreviewPanel;
    public InputField PlanetNameField;
    public Toggle PlanetRingToggle;
    public Slider PlanetColourRedSlider;
    public Slider PlanetColourBlueSlider;
    public Slider PlanetColourGreenSlider;
    public Slider PlanetSizeSlider;
    public PlayerManager Player;
    public float WorldSizeMod = 10;
    public float WorldSpawnDistance = 10;

    private Dictionary<string,PlanetHolder> PlanetDictionary = new Dictionary<string, PlanetHolder>();
    public void AttemptCreatePlanet()
    {
        var position = Player.transform.position + Player.transform.forward * WorldSpawnDistance;
        var planetName = PlanetNameField.text;
        if (planetName.Trim() == string.Empty)
            return;

        if (planetName.Length > 140)
            return;

        var color = new Color
        {
            r = PlanetColourRedSlider.value,
            g = PlanetColourGreenSlider.value,
            b = PlanetColourBlueSlider.value
        };

        var size = PlanetSizeSlider.value * WorldSizeMod;
        if (size < 0.000001)
        {
            return;
        }

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
            Size = size,
            ColourRed = color.r,
            ColourGreen = color.g,
            ColourBlue = color.b,
            Type = planetType,
            CreatedByUserId = Player.Player.Id,
            CreatedByUsername = Player.Player.Username
        };

        if (Physics.CheckSphere(position, planet.Size))
        {
            return;
        }

        SpawnPlanet(planet);
        SavePlanet(planet);
        PlanetCreationPanel.SetActive(false);
        Cursor.visible = false;
    }

    public void SpawnPlanet(Planet planet)
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

        var color = new Color
        {
            r = planet.ColourRed,
            g = planet.ColourGreen,
            b = planet.ColourBlue
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

        PlanetDictionary.Add(planet.Id, planetHolder);

    }

    private void SavePlanet(Planet planet)
    {
        StartCoroutine(SavePlanetCoroutine(planet));
    }

    private IEnumerator SavePlanetCoroutine(Planet planet)
    {
        var headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");

        Debug.Log(planet.CreatedByUserId);

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
            yield return new WaitForSeconds(60);
        }
    }

    private DateTime lastFetchTime = DateTime.MinValue;
    private IEnumerator PlanetRecentLoadingLoop()
    {
        while (true)
        {
            yield return GetPlanets(lastFetchTime);
            lastFetchTime = DateTime.Now;
            yield return new WaitForSeconds(5);
        }
    }

    private IEnumerator GetPlanets(DateTime dateFrom)
    {
        int page = 0;
        bool finished = false;
        while (!finished)
        {
            var encodedDate = WWW.EscapeURL(dateFrom.ToString());
            Debug.Log(encodedDate);
            var www = new WWW(WebApiAccess.ApiUrl + "/Planet?dateFrom=" + encodedDate + "&page=" + page);
            yield return www;

            var jsonList = new JSONObject(www.text);
            if (jsonList.list != null)
            {
                foreach (var jsonPlanet in jsonList)
                {
                    var planet = JsonToPlanet(jsonPlanet);
                    SpawnPlanet(planet);
                    yield return new WaitForEndOfFrame();
                }
            }

            if (jsonList.list == null || jsonList.list.Count == 0)
            {
                finished = true;
            }
        }
    }

    private Planet JsonToPlanet(JSONObject json)
    {
        Debug.Log(json.ToString());
        var elementObject = json.ToDictionary();
        Planet parsedPlanet = new Planet();
        parsedPlanet.Id = elementObject["id"];
        parsedPlanet.Name = elementObject["name"];
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
