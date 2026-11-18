using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("TutorialPlayed") || PlayerPrefs.GetInt("TutorialPlayed") <= 1)
        {
            //Run tutorial!
            PlayerPrefs.SetInt("TutorialPlayed", 1);
        }
        else
        {
            //Skip tutorial
            PlayerPrefs.SetInt("TutorialPlayed", 2);
        }
    }
}
