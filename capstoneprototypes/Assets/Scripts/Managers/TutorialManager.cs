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
    GameDataManager gdm;
    public Button nextRoundBtn;
    public Transform tutorialUI;
    public GameObject tutorialUI_0;
    public GameObject tutorialUI_1;
    public GameObject tutorialUI_2;
    public GameObject tutorialUI_3;
    public GameObject tutorialUI_4;

    void Start()
    {
        gdm = GameObject.Find("GameDataManager").GetComponent<GameDataManager>();

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
                case 8: progress++; break;
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

            if (progress >= 5) 
            { nextRoundBtn.interactable = true; nextRoundBtn.transform.GetComponent<SpriteChangeOnMouse>().enabled = true; }
            else { nextRoundBtn.interactable = false; nextRoundBtn.transform.GetComponent<SpriteChangeOnMouse>().enabled = false; }

            if (progress == 6 || progress == 7) { gdm.timeline.combatSpeed = 0; } else if (gdm.timeline.combatSpeed == 0) { gdm.timeline.combatSpeed = 1; }

            yield return new WaitForEndOfFrame();
        }
        gdm.timeline.combatSpeed = 1;

        progress = 0;
        waitingFor = 0;
        nextRoundBtn.interactable = true;
        nextRoundBtn.transform.GetComponent<SpriteChangeOnMouse>().enabled = true;
        tutorialUI_0.SetActive(false);
        tutorialUI.gameObject.SetActive(false);

        yield return null;
    }
}
