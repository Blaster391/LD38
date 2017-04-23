using UnityEngine;
using System.Collections;

public class ExitScript : MonoBehaviour
{

    public GameObject ExitMenu;

    void Start()
    {
        ExitMenu.SetActive(false);
    }
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.Escape))
	    {
	        ShowHideExitMenu();
	    }
	}

    public void ShowHideExitMenu()
    {
        if (ExitMenu.activeSelf)
        {
            Cursor.visible = false;
            ExitMenu.SetActive(false);
        }
        else
        {
            Cursor.visible = true;
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
