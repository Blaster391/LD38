using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class PlanetManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Player = GameObject.Find("Player").GetComponent<PlayerManager>();
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
    public Slider PlanetColourRedSlider;
    public Slider PlanetColourBlueSlider;
    public Slider PlanetColourGreenSlider;
    public Slider PlanetSizeSlider;
    public PlayerManager Player;
    public float WorldSizeMod = 10;
    public float WorldSpawnDistance = 10;
    public void AttemptCreatePlanet()
    {
        var position = Player.transform.position + Player.transform.forward * WorldSpawnDistance;

        var color = new Color
        {
            r = PlanetColourRedSlider.value,
            g = PlanetColourGreenSlider.value,
            b = PlanetColourBlueSlider.value
        };

        var size = PlanetSizeSlider.value * WorldSizeMod;

        var planet = new Planet
        {
            Id = "PLANET." + Guid.NewGuid(),
            Name = "Steve",
            PositionX = position.x,
            PositionY = position.y,
            PositionZ = position.z,
            Size = size,
            ColourRed = color.r,
            ColourGreen = color.g,
            ColourBlue = color.b,

            CreateByUserId = Player.Player.Id,
            CreateByUsername = Player.Player.Username
        };

        if (Physics.CheckSphere(position, planet.Size))
        {
            return;
        }

        SpawnPlanet(planet);
        SavePlanet(planet);
    }

    public void SpawnPlanet(Planet planet)
    {
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

        PlanetCreationPanel.SetActive(false);
    }

    private void SavePlanet(Planet planet)
    {
        //TODO this shit lol



    }
}
