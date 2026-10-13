using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Combat");
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenLinkURL(string link)
    {
        Application.OpenURL(link);
    }
}
