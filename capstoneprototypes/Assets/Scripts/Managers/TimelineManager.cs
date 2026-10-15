using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UI;
using static TimelineManager;

public class TimelineManager : MonoBehaviour
{
    [System.Serializable]
    public class TimelineEvent
    {
        public EventType eventType;
        public int timelineIndex;
        public RectTransform markerTrans;
        public bool removed = false;

        //Enemy event
        public Enemy enemyOwner;
        public Enemy.Action enemyAction;

        //Player skill
        public PlayerSkill playerSkill;
        public Enemy enemyTargeted;
        public GameObject skillBlock;
    }
    public enum EventType { EnemyAtk, PlayerAtk, SpeedUp, SpeedDown, None, ReverseStart, ReverseEnd, PortalStart, PortalEnd }

    [Header("Timekeeping")]
    public float combatTime;
    public float timePassed;
    public float prevTimePassed;
    int prevEventIndex;
    public int eventIndex;
    public int roundNumber;
    public float combatSpeed;
    public bool combatActive; //is there currently a combat happening
    public bool combatPause; //is the real-time paused or active
    float metronomeTimer = 1f;

    [Header("Visuals")]
    public Vector2 minMaxPos;
    public bool fillPixelByPixel;
    public float resolution;

    [Header("Functional")]
    public List<TimelineEvent> eventList;
    public List<Enemy> activeEnemies;

    [Header("References")]
    public CameraGlide camGlide;
    public GameObject spotlight;
    public Image fillBar;
    public GameObject markerPrefab;
    public GameObject skillBlockPrefab;
    PlayerActionManager player;
    public Transform thresholdBar;
    AudioManager audioMan;
    public RoomSelection roomSelection;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerActionManager>();
        audioMan = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    private void Start()
    {
        combatActive = false;
        roomSelection.gameObject.SetActive(false);
        combatPause = true;
        markerPrefab.SetActive(false);

        StartCombat();
    }
    private void Update()
    {
        if (combatActive && !combatPause) { UpdateCombat(); }
        else { UpdateVisuals(); }
        UpdateMetronome();
    }
    void UpdateVisuals()
    {
        if (combatActive) { spotlight.SetActive(combatPause); } else { spotlight.SetActive(false); }

        if (fillPixelByPixel)
        {
            fillBar.fillAmount = eventIndex / resolution;
        }
        else
        {
            fillBar.fillAmount = timePassed / combatTime;
        }

        thresholdBar.transform.localPosition = new Vector3(Mathf.Lerp(minMaxPos.x, minMaxPos.y, fillBar.fillAmount), 0, 0);
    }
    public void StartCombat()
    {
        if (combatActive) { return; }
        combatActive = true;
        combatPause = true;
        roundNumber = 0;

        eventList = new List<TimelineEvent>();
        timePassed = 0;
        prevEventIndex = -1;

        PrepareNextRound();
        foreach (Enemy enemy in activeEnemies) { enemy.SetupMarkers(); }

        player.energy = player.maxEnergy;
        player.SkillSelectStart();

        camGlide.isUp = false;
    }
    public void StartRound()
    {
        if (!combatPause) { return; }
        if (!combatActive) { return; }
        combatPause = false;

        camGlide.isUp = true;
    }
    void EndRound()
    {
        combatPause = true;
        timePassed = 0;
        prevEventIndex = -1;
        roundNumber++;

        foreach (TimelineEvent e in eventList)
        {
            Destroy(e.markerTrans.gameObject);
            if (e.skillBlock != null) { Destroy(e.skillBlock); }
            e.removed = true;
        }

        eventList = new List<TimelineEvent>();

        //Check if all enemies are dead. If they are, then end the combat.
        bool combatOver = true;
        foreach (Enemy e in activeEnemies)
        {
            if (e.hp > 0) { combatOver = false; break; }
        }
        if (combatOver) { EndCombat(); return; }

        PrepareNextRound();
        foreach (Enemy enemy in activeEnemies) { enemy.SetupMarkers(); }

        player.SkillSelectStart();

        camGlide.isUp = false;
    }
    void PrepareNextRound()
    {
        int highestPriority = -1;
        Enemy.AttackPattern priorityEnemyPattern = null;
        foreach (Enemy enemy in activeEnemies)
        {
            if (enemy.hp > 0)
            {
                Enemy.AttackPattern tempPattern = new Enemy.AttackPattern();
                tempPattern = enemy.GetPatternFromRound(roundNumber);

                if (tempPattern.timelineChangePriority > highestPriority) 
                { 
                    highestPriority = tempPattern.timelineChangePriority;
                    priorityEnemyPattern = tempPattern;
                }

                enemy.PrepareForNextRound();
            }
        }
        if (priorityEnemyPattern != null)
        {
            combatTime = priorityEnemyPattern.timelineCombatTimePref;
            combatSpeed = priorityEnemyPattern.timelineCombatSpeedPref;
        }
        else
        {
            combatTime = 6f;
            combatSpeed = 1f;
        }
    }
    public void EndCombat()
    {
        //Clean up the rest of the events
        foreach (TimelineEvent e in eventList)
        {
            Destroy(e.markerTrans.gameObject);
            if (e.skillBlock != null) { Destroy(e.skillBlock); }
            e.removed = true;
        }
        for (int i = 0; i < eventList.Count; i++)
        {
            eventList.RemoveAt(0);
        }

        combatActive = false;
        combatPause = true;
        timePassed = 0;
        roundNumber = 0;
        prevEventIndex = -1;
        combatSpeed = 0;

        camGlide.isUp = true;

        roomSelection.gameObject.SetActive(true);
    }
    void UpdateCombat()
    {
        eventIndex = Mathf.FloorToInt((timePassed / combatTime) * resolution);

        //Don't run the same event index multiple times but run all events that still need to be run
        int catchUp = eventIndex - prevEventIndex;
        if (eventIndex != prevEventIndex)
        {
            foreach (TimelineEvent e in eventList)
            {
                if (!e.removed && (e.timelineIndex == eventIndex || (e.timelineIndex < eventIndex && e.timelineIndex > eventIndex-catchUp))) { RunEvent(e); }
            }
        }

        foreach (Enemy enemy in activeEnemies) { enemy.TimelineUpdate(); }

        UpdateVisuals();
        prevTimePassed = timePassed;
        timePassed += Time.deltaTime * combatSpeed;
        prevEventIndex = eventIndex;

        if (eventIndex > resolution) { EndRound(); }
    }
    TimelineEvent mostRecentAddedEvent = null;
    public void AddMarker(int index, EventType eventType, Enemy enemyOwner, Enemy.Action enemyAction, PlayerSkill playerSkill, Enemy enemyTargeted)
    {
        TimelineEvent newEvent = new TimelineEvent();
        newEvent.eventType = eventType;
        newEvent.timelineIndex = index;
        newEvent.enemyOwner = enemyOwner;
        newEvent.enemyAction = enemyAction;
        newEvent.playerSkill = playerSkill;
        newEvent.enemyTargeted = enemyTargeted;
        newEvent.skillBlock = null;

        RectTransform newMarker = null;
        if ((playerSkill != null && playerSkill.perfectable) || playerSkill == null)
        {
            newMarker = Instantiate(markerPrefab, transform.GetChild(0)).GetComponent<RectTransform>();
            MarkerObject markObj = newMarker.GetComponent<MarkerObject>();
            markObj.SetType(eventType);
            newMarker.transform.localPosition = new Vector3(Mathf.Lerp(minMaxPos.x, minMaxPos.y, index / resolution), 0, 0);
            newMarker.gameObject.SetActive(true);
        }

        newEvent.markerTrans = newMarker;
        
        eventList.Add(newEvent);

        mostRecentAddedEvent = newEvent;
    }
    public void MoveEvent(TimelineEvent tEvent, int newIndex)
    {
        tEvent.timelineIndex = newIndex;

        Vector3 newPos = new Vector3(Mathf.Lerp(minMaxPos.x, minMaxPos.y, newIndex / resolution), 0, 0);
        if (tEvent.markerTrans != null)
        {
            tEvent.markerTrans.transform.localPosition = newPos;
        }
        if (tEvent.skillBlock != null)
        {
            Vector3 newInitialPos = new Vector3(newPos.x - IndexFromTime(tEvent.playerSkill.prepTime), newPos.y, newPos.z);
            tEvent.skillBlock.transform.localPosition = newInitialPos;
        }
    }
    public void CondensePlayerSkillEvents()
    {
        //Get all player events
        List<TimelineEvent> playerEvents = new List<TimelineEvent>();
        List<int> eventStarts = new List<int>();

        foreach (TimelineEvent tEvent in eventList)
        {
            if (tEvent.eventType == EventType.PlayerAtk && !tEvent.removed) 
            {
                eventStarts.Add(tEvent.timelineIndex - IndexFromTime(tEvent.playerSkill.prepTime));
                playerEvents.Add(tEvent);
            }
        }

        //Sort the player events
        for (int y = 0; y < playerEvents.Count; y++)
        {
            int earliestEventIndex = -1;
            int earliestEventTime = 999999;
            for (int i = y; i < playerEvents.Count; i++)
            {
                if (eventStarts[i] < earliestEventTime) { earliestEventIndex = i; earliestEventTime = eventStarts[i]; }
            }
            if (earliestEventIndex != -1)
            {
                int oldIndex = earliestEventIndex;
                //Rearrange player events to put the earliest at the start
                playerEvents.Add(null);
                playerEvents[playerEvents.Count - 1] = playerEvents[y];
                playerEvents[y] = playerEvents[oldIndex];
                playerEvents[oldIndex] = playerEvents[playerEvents.Count - 1];
                playerEvents.RemoveAt(playerEvents.Count - 1);

                //Rearrange event starts to put the earliest at the start
                eventStarts.Add(-1);
                eventStarts[eventStarts.Count - 1] = eventStarts[y];
                eventStarts[y] = eventStarts[oldIndex];
                eventStarts[oldIndex] = eventStarts[eventStarts.Count - 1];
                eventStarts.RemoveAt(eventStarts.Count - 1);
            }
            else { break; } //If nothing earlier was found, list is sorted.
        }

        //Move the player events into place
        for (int i = 0; i < playerEvents.Count; i++)
        {
            if (i == 0) 
            {
                MoveEvent(playerEvents[i], playerEvents[i].timelineIndex - eventStarts[i]);
            }
            else
            {
                int dist = eventStarts[i] - playerEvents[i - 1].timelineIndex;
                MoveEvent(playerEvents[i], playerEvents[i].timelineIndex - dist);
            }
        }
    }
    public void AddSkillBlock(int index, int timePassedIndex)
    {
        SkillBlock newBlock = Instantiate(skillBlockPrefab, transform.GetChild(0)).GetComponent<SkillBlock>();
        Vector3 initialPos = new Vector3(Mathf.Lerp(minMaxPos.x, minMaxPos.y, timePassedIndex / resolution), 0, 0);
        Vector3 endPos = new Vector3(Mathf.Lerp(minMaxPos.x, minMaxPos.y, index / resolution), 0, 0);
        newBlock.SetUpBlock(initialPos, endPos, mostRecentAddedEvent);
        newBlock.gameObject.SetActive(true);

        mostRecentAddedEvent.skillBlock = newBlock.gameObject;
    }
    public void PreviewSkill(int index, int timePassedIndex, PlayerSkill skill, bool skillIsPlaceable)
    {
        skillBlockPrefab.SetActive(true);
        SkillBlock previewBlock = skillBlockPrefab.GetComponent<SkillBlock>();
        Vector3 initialPos = new Vector3(Mathf.Lerp(minMaxPos.x, minMaxPos.y, timePassedIndex / resolution), 0, 0);
        Vector3 endPos = new Vector3(Mathf.Lerp(minMaxPos.x, minMaxPos.y, index / resolution), 0, 0);
        previewBlock.SetUpBlock(initialPos, endPos, null);

        if (!skillIsPlaceable) {  }

        if (skill.perfectable)
        {
            markerPrefab.SetActive(true);
            MarkerObject markObj = markerPrefab.GetComponent<MarkerObject>();
            markObj.SetType(EventType.PlayerAtk);
            markerPrefab.transform.localPosition = new Vector3(Mathf.Lerp(minMaxPos.x, minMaxPos.y, index / resolution), 0, 0);
        }
    }
    public void StopPreviewSkillBlock()
    {
        skillBlockPrefab.SetActive(false);
        markerPrefab.SetActive(false);
    }
    void RunEvent(TimelineEvent tEvent)
    {
        switch (tEvent.eventType)
        {
            case EventType.EnemyAtk: tEvent.enemyOwner.Attack(tEvent); break;
            case EventType.PlayerAtk: player.StartCoroutine(player.ReactableEvent(tEvent)); break;
            case EventType.SpeedUp: combatSpeed += tEvent.enemyAction.damage; break;
            case EventType.SpeedDown: combatSpeed -= tEvent.enemyAction.damage; break;
            case EventType.None: break;
            case EventType.ReverseStart: break;
            case EventType.ReverseEnd: break;
            case EventType.PortalStart: break;
            case EventType.PortalEnd: break;
        }
    }
    public void EnemyDied(Enemy enemy)
    {
        foreach (TimelineEvent tEvent in eventList)
        {
            if (tEvent.enemyOwner == enemy) { RemoveEventAndMarker(tEvent, false); }
        }
    }
    public void RemoveEventAndMarker(TimelineEvent tEvent, bool playerRemoved)
    {
        //Doesn't REMOVE the event and marker so that everything can run smoothly.
        //Instead, disable that event, and hide the markers for it.
        //It will be cleaned up when the round ends anyways.
        tEvent.removed = true;
        if (tEvent.markerTrans != null) { tEvent.markerTrans.gameObject.SetActive(false); }
        if (tEvent.skillBlock != null) { tEvent.skillBlock.SetActive(false); }
        if (playerRemoved) { player.SkillRemoved(tEvent); CondensePlayerSkillEvents(); }
    }
    public float TimeFromIndex(int index)
    {
        float result = 0;
        result = (index / resolution) * combatTime;
        return result;
    }
    public int IndexFromTime(float time)
    {
        int result = 0;
        result = Mathf.FloorToInt((time / combatTime) * resolution);
        return result;
    }
    AudioManager.SFXSource metronomeSFX;
    void UpdateMetronome()
    {
        metronomeTimer -= Time.deltaTime * combatSpeed;
        if (metronomeTimer <= 0) 
        { 
            metronomeTimer += 1;

            //If it is already playing, stop it. Commented out because metronome should stack when fast?
            //if (audioMan.activeSources.Contains(metronomeSFX)) { audioMan.StopSound(metronomeSFX); }

            metronomeSFX = audioMan.PlaySound(0, AudioManager.SFXType.Effect, 1f, combatSpeed);
        }
    }
}
