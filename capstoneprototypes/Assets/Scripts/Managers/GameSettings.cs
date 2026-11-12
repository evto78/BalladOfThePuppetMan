using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public Toggle isFullscreen;
    public GameObject settingsScreen;
    bool settingsOpen = false;
    bool timeChanged = false;

    private void Start()
    {
        isFullscreen.isOn = PlayerPrefs.GetInt("Fullscreen") == 1;
        Screen.fullScreen = isFullscreen.isOn;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            settingsOpen = !settingsOpen;
            settingsScreen.SetActive(settingsOpen);
            timeChanged = false;
        }

        if (settingsOpen && !timeChanged)
        {
            Time.timeScale = 0f;
            timeChanged = true;
        }
        else if (!settingsOpen && !timeChanged)
        {
            Time.timeScale = 1f;
            timeChanged = true;
        }
    }

    public void ChangeScreenState()
    {
        Screen.fullScreen = isFullscreen.isOn;

        PlayerPrefs.SetInt("Fullscreen", isFullscreen.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }
}
