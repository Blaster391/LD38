using UnityEngine;
using System.Collections;

public class ExitScript : MonoBehaviour
{

    public GameObject ExitMenu;
	
	// Update is called once per frame
	void Update () {
        ExitMenu.SetActive(false);
        if (Input.GetKeyUp(KeyCode.Escape))
	    {
	        ShowHideExitMenu();
	    }
	}

    public void ShowHideExitMenu()
    {
        if (ExitMenu.activeSelf)
        {
            ExitMenu.SetActive(false);
        }
        else
        {
            ExitMenu.SetActive(true);
        }

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
