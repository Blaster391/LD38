using UnityEngine;
using System.Collections;

public class TextLookScript : MonoBehaviour
{

    public GameObject Player;
	// Use this for initialization
	void Start () {
        Player = GameObject.Find("Player");

    }
	
	// Update is called once per frame
	void Update () {
	    transform.LookAt(Player.transform);
        gameObject.transform.Rotate(new Vector3(180,0,180));
	}
}
