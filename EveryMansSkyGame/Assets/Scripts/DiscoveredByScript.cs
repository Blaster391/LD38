using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DiscoveredByScript : MonoBehaviour
{

    public float Speed;
    public float LiveTime;
    public Text PlanetName;
    public Text PlayerName;


    public void Begin(string planetName, string playerName)
    {
        var canvas = GameObject.Find("Canvas");
        gameObject.transform.parent = canvas.transform;
        gameObject.transform.localPosition = new Vector3(0,0,0);

        PlanetName.text = planetName;
        PlayerName.text = "By " + playerName;
        StartCoroutine(Animate());
        StartCoroutine(DestroyTimer());
    }

    IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(LiveTime);
        Destroy(gameObject);
    }

    IEnumerator Animate()
    {
        while (true)
        {
            PlayerName.transform.Translate(Vector3.left * Speed * Time.deltaTime);
            PlanetName.transform.Translate(Vector3.right * Speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
}
