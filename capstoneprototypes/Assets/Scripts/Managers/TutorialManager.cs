using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("Functional")]
    public int progress = 0;
    public int waitingFor;

    [Header("References")]
    public Button nextRoundBtn;
    public Transform tutorialUI;
    public GameObject tutorialUI_0;
    public GameObject tutorialUI_1;
    public GameObject tutorialUI_2;
    public GameObject tutorialUI_3;
    public GameObject tutorialUI_4;

    void Start()
    {
        waitingFor = 0;
        progress = 0;

        tutorialUI.gameObject.SetActive(false);
        tutorialUI_0.SetActive(false);
        tutorialUI_1.SetActive(false);
        tutorialUI_2.SetActive(false);
        tutorialUI_3.SetActive(false);
        tutorialUI_4.SetActive(false);

        CheckTutorial();
    }

    public bool CheckTutorial()
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

        return PlayerPrefs.GetInt("TutorialPlayed") <= 1;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            switch (waitingFor)
            {
                case 4: progress++; break;
                case 5: progress++; break;
                case 7: progress++; break;
            }
        }
    }

    public void PlayTutorial(int index)
    {
        switch (index)
        {
            case 0: StartCoroutine(Tutorial_0()); break;
            case 1: break;
            case 2: break;
            case 3: break;
            case 4: break;
        }
    }

    IEnumerator Tutorial_0()
    {
        progress = 0;
        nextRoundBtn.interactable = false;
        nextRoundBtn.transform.GetComponent<SpriteChangeOnMouse>().enabled = false;
        tutorialUI.gameObject.SetActive(true);
        tutorialUI_0.SetActive(true);
        tutorialUI_1.SetActive(false);
        tutorialUI_2.SetActive(false);
        tutorialUI_3.SetActive(false);
        tutorialUI_4.SetActive(false);

        for (int i = 0; i < tutorialUI_0.transform.GetChild(0).childCount; i++) { tutorialUI_0.transform.GetChild(0).GetChild(i).gameObject.SetActive(false); }

        waitingFor = 1;
        while (progress < tutorialUI_0.transform.GetChild(0).childCount)
        {
            for (int i = 0; i < tutorialUI_0.transform.GetChild(0).childCount; i++) { tutorialUI_0.transform.GetChild(0).GetChild(i).gameObject.SetActive(false); }
            tutorialUI_0.transform.GetChild(0).GetChild(progress).gameObject.SetActive(true);

            waitingFor = progress + 1;

            if (progress == 6) 
            { nextRoundBtn.interactable = true; nextRoundBtn.transform.GetComponent<SpriteChangeOnMouse>().enabled = true; }
            else { nextRoundBtn.interactable = false; nextRoundBtn.transform.GetComponent<SpriteChangeOnMouse>().enabled = false; }

            yield return new WaitForEndOfFrame();
        }

        progress = 0;
        waitingFor = 0;
        nextRoundBtn.interactable = true;
        nextRoundBtn.transform.GetComponent<SpriteChangeOnMouse>().enabled = true;
        tutorialUI_0.SetActive(false);
        tutorialUI.gameObject.SetActive(false);

        yield return null;
    }
}
