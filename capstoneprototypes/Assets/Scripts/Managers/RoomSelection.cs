using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class RoomSelection : MonoBehaviour
{
    public List<Sprite> roomIcons;
    public List<Sprite> selectedRoomIcons;
    public Sprite thoughtBubbleSprite;
    public Sprite selectedThoughtBubbleSprite;
    public List<SpriteRenderer> optionSprites;
    public Sprite sunIcon;
    public Sprite moonIcon;
    public List<SpriteRenderer> timeIcons;
    public List<Vector3> paths = new List<Vector3>();
    public int distance;
    GameDataManager gdm;
    void Start()
    {
        gdm = GameObject.Find("GameDataManager").GetComponent<GameDataManager>();

        distance = 0;
        if (paths.Count < 5) { GeneratePaths(); }

        DisplayPaths();
    }
    void GeneratePaths()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 newPath; 

            if (paths.Count > 6)
            {
                //Past the boss encounter, just place question marks as a placeholder
                newPath = new Vector3(-1.1f, 1.1f, -1.1f);
                if (Random.Range(0, 2) == 0) { newPath = new Vector3(-newPath.x, newPath.y, newPath.z); }
                if (Random.Range(0, 2) == 0) { newPath = new Vector3(newPath.x, -newPath.y, newPath.z); }
                if (Random.Range(0, 2) == 0) { newPath = new Vector3(newPath.x, newPath.y, -newPath.z); }
            }
            else if (paths.Count == 6) 
            {
                //Force the boss encounter on the 6th room
                newPath = new Vector3(-(roomIcons.Count - 1) + 0.1f, (roomIcons.Count - 1) + 0.1f, -(roomIcons.Count - 1) + 0.1f);
            }
            else
            {
                //For all other rooms, pick a random, NONBOSS room
                newPath = new Vector3(Random.Range(0, roomIcons.Count-1) + 0.1f, Random.Range(0, roomIcons.Count-1) + 0.1f, Random.Range(0, roomIcons.Count-1) + 0.1f);
                if (Random.Range(0, 2) == 0) { newPath = new Vector3(-newPath.x, newPath.y, newPath.z); }
                if (Random.Range(0, 2) == 0) { newPath = new Vector3(newPath.x, -newPath.y, newPath.z); }
                if (Random.Range(0, 2) == 0) { newPath = new Vector3(newPath.x, newPath.y, -newPath.z); }
            }

                paths.Add(newPath);
        }
    }
    void DisplayPaths()
    {
        if (paths[distance].x < 0) { timeIcons[0].sprite = moonIcon; } else { timeIcons[0].sprite = sunIcon; }
        if (paths[distance].y < 0) { timeIcons[1].sprite = moonIcon; } else { timeIcons[1].sprite = sunIcon; }
        if (paths[distance].z < 0) { timeIcons[2].sprite = moonIcon; } else { timeIcons[2].sprite = sunIcon; }

        if (IsTouchingMouse(optionSprites[0].gameObject))
        { optionSprites[0].sprite = selectedThoughtBubbleSprite; optionSprites[0].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = selectedRoomIcons[Mathf.Abs(Mathf.RoundToInt(paths[distance].x))]; }
        else { optionSprites[0].sprite = thoughtBubbleSprite; optionSprites[0].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = roomIcons[Mathf.Abs(Mathf.RoundToInt(paths[distance].x))]; }

        if (IsTouchingMouse(optionSprites[1].gameObject))
        { optionSprites[1].sprite = selectedThoughtBubbleSprite; optionSprites[1].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = selectedRoomIcons[Mathf.Abs(Mathf.RoundToInt(paths[distance].y))]; }
        else { optionSprites[1].sprite = thoughtBubbleSprite; optionSprites[1].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = roomIcons[Mathf.Abs(Mathf.RoundToInt(paths[distance].y))]; }

        if (IsTouchingMouse(optionSprites[2].gameObject))
        { optionSprites[2].sprite = selectedThoughtBubbleSprite; optionSprites[2].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = selectedRoomIcons[Mathf.Abs(Mathf.RoundToInt(paths[distance].z))]; }
        else { optionSprites[2].sprite = thoughtBubbleSprite; optionSprites[2].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = roomIcons[Mathf.Abs(Mathf.RoundToInt(paths[distance].z))]; }

        //NEXT display 1
        if (paths[distance + 1].x < 0) { timeIcons[3].sprite = moonIcon; } else { timeIcons[3].sprite = sunIcon; }
        if (paths[distance + 1].y < 0) { timeIcons[4].sprite = moonIcon; } else { timeIcons[4].sprite = sunIcon; }
        if (paths[distance + 1].z < 0) { timeIcons[5].sprite = moonIcon; } else { timeIcons[5].sprite = sunIcon; }

        
        optionSprites[3].sprite = thoughtBubbleSprite; optionSprites[3].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = roomIcons[Mathf.Abs(Mathf.RoundToInt(paths[distance + 1].x))];
        optionSprites[4].sprite = thoughtBubbleSprite; optionSprites[4].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = roomIcons[Mathf.Abs(Mathf.RoundToInt(paths[distance + 1].y))];
        optionSprites[5].sprite = thoughtBubbleSprite; optionSprites[5].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = roomIcons[Mathf.Abs(Mathf.RoundToInt(paths[distance + 1].z))];

        //NEXT display 2
        if (paths[distance + 2].x < 0) { timeIcons[6].sprite = moonIcon; } else { timeIcons[6].sprite = sunIcon; }
        if (paths[distance + 2].y < 0) { timeIcons[7].sprite = moonIcon; } else { timeIcons[7].sprite = sunIcon; }
        if (paths[distance + 2].z < 0) { timeIcons[8].sprite = moonIcon; } else { timeIcons[8].sprite = sunIcon; }


        optionSprites[6].sprite = thoughtBubbleSprite; optionSprites[6].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = roomIcons[Mathf.Abs(Mathf.RoundToInt(paths[distance + 2].x))];
        optionSprites[7].sprite = thoughtBubbleSprite; optionSprites[7].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = roomIcons[Mathf.Abs(Mathf.RoundToInt(paths[distance + 2].y))];
        optionSprites[8].sprite = thoughtBubbleSprite; optionSprites[8].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = roomIcons[Mathf.Abs(Mathf.RoundToInt(paths[distance + 2].z))];

        //NEXT display 3
        if (paths[distance + 3].x < 0) { timeIcons[9].sprite = moonIcon; } else { timeIcons[9].sprite = sunIcon; }
        if (paths[distance + 3].y < 0) { timeIcons[10].sprite = moonIcon; } else { timeIcons[10].sprite = sunIcon; }
        if (paths[distance + 3].z < 0) { timeIcons[11].sprite = moonIcon; } else { timeIcons[11].sprite = sunIcon; }


        optionSprites[9].sprite = thoughtBubbleSprite; optionSprites[9].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = roomIcons[Mathf.Abs(Mathf.RoundToInt(paths[distance + 3].x))];
        optionSprites[10].sprite = thoughtBubbleSprite; optionSprites[10].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = roomIcons[Mathf.Abs(Mathf.RoundToInt(paths[distance + 3].y))];
        optionSprites[11].sprite = thoughtBubbleSprite; optionSprites[11].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = roomIcons[Mathf.Abs(Mathf.RoundToInt(paths[distance + 3].z))];
    }
    public bool IsTouchingMouse(GameObject g)
    {
        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return g.GetComponent<Collider2D>().OverlapPoint(point);
    }
    void Update()
    {
        DisplayPaths();

        if (Input.GetMouseButtonDown(0))
        {
            if (IsTouchingMouse(optionSprites[0].gameObject)) { SelectRoom(0); }
            else if (IsTouchingMouse(optionSprites[1].gameObject)) { SelectRoom(1); }
            else if (IsTouchingMouse(optionSprites[2].gameObject)) { SelectRoom(2); }
        }
    }
    void SelectRoom(int option)
    {
        //Go to the selected room!
        float roomData = paths[distance].x;
        switch (option)
        {
            case 0: roomData = paths[distance].x; break;
            case 1: roomData = paths[distance].y; break;
            case 2: roomData = paths[distance].z; break;
        }
        switch (Mathf.Abs(Mathf.RoundToInt(roomData)))
        {
            case 0: //COMBAT
                gdm.enemySpawnMan.SpawnRandomEncounter();
                gdm.ChangeState(GameDataManager.State.StartCombat); 
                break; 

            case 1: //EVENT
                break; 

            case 2: //HEAL
                gdm.player.HealRoom(); 
                break;

            case 3: //BOSS
                gdm.enemySpawnMan.SpawnEncounter(6); //Run encounter 6, which is the tonguetwister boss encounter.
                gdm.ChangeState(GameDataManager.State.StartCombat);
                break;
        }

        distance++;

        if (distance >= paths.Count-5)
        {
            GeneratePaths();
        }

        DisplayPaths();
    }
}
