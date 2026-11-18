using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject tutorialPopUp;
    public GameObject buttonsHolder;

    private void Start()
    {
        buttonsHolder.SetActive(true);
        tutorialPopUp.SetActive(false);
    }

    public void StartGame()
    {
        if (!PlayerPrefs.HasKey("TutorialPlayed") || PlayerPrefs.GetInt("TutorialPlayed") == 0)
        {
            tutorialPopUp.SetActive(true);
            buttonsHolder.SetActive(false);
        }
        else
        {
            SceneManager.LoadScene("Combat");
        }
    }
    public void TutorialSet(int set)
    {
        PlayerPrefs.SetInt("TutorialPlayed", set);
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
