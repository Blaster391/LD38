using UnityEngine;
using System.Collections;

public class ExitScript : MonoBehaviour
{

    public GameObject ExitMenu;
	
	// Update is called once per frame
	void Update () {

	    if (Input.GetKeyUp(KeyCode.Escape))
	    {
	        ShowExitMenu();
	    }
	}

    public void ShowExitMenu()
    {
        ExitMenu.SetActive(true);
    }

    public void HideExitMenu()
    {
        ExitMenu.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
