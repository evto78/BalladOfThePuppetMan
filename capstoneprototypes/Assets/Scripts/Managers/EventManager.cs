using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [Header("References")]
    public List<GameObject> eventObjects;
    public GameObject perspectiveCam;
    public List<GameObject> BGLayers;
    GameDataManager gdm;

    private void Awake()
    {
        gdm = GameObject.Find("GameDataManager").GetComponent<GameDataManager>();
    }

    public void StartEvent(int eventID)
    {
        if (!waited) { StartCoroutine(WaitToStart(eventID)); return; }

        switch (eventID)
        {
            case 0: //TakeOneLeaveOne Event
                eventObjects[0].SetActive(true); 
                break;

            case 1: //BackStage Event
                perspectiveCam.SetActive(true);
                eventObjects[1].SetActive(true);
                eventObjects[1].GetComponentInChildren<Animator>().SetBool("IsUp", false);
                foreach (GameObject obj in BGLayers) { obj.SetActive(false); }
                break; 
        }
    }

    bool waited = false;
    IEnumerator WaitToStart(int eventID)
    {
        yield return new WaitForSeconds(1f);
        waited = true;
        StartEvent(eventID);
        waited = false;
    }

    public void EndEvent(int eventID)
    {
        switch (eventID)
        {
            case 0: //TakeOneLeaveOne Event
                eventObjects[0].SetActive(false);
                break;

            case 1: //BackStage Event
                perspectiveCam.SetActive(false);
                eventObjects[1].GetComponentInChildren<Animator>().SetBool("IsUp", true);
                foreach (GameObject obj in BGLayers) { obj.SetActive(true); }
                eventObjects[1].SetActive(false);
                break;
        }

        gdm.ChangeState(GameDataManager.State.RoomSelection);
    }
}
