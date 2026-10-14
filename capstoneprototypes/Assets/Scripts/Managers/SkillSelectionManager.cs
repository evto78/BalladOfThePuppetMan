using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillSelectionManager : MonoBehaviour
{
    public List<GameObject> skillButtons;
    public List<TextMeshProUGUI> energyCost;
    public List<TextMeshProUGUI> skillText;
    public List<GameObject> energyIcon;
    public TextMeshProUGUI readout;
    public TextMeshProUGUI energyText;
    PlayerActionManager player;

    public List<ReadoutOnHover> readoutObjects;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerActionManager>();
    }

    void Update()
    {
        bool hovering = false;
        player.timeline.StopPreviewSkillBlock();
        for (int i = 0; i < skillButtons.Count; i++)
        {
            if (IsTouchingMouse(skillButtons[i])) 
            {
                hovering = true;
                readout.text = player.equippedSkills[i].skillDescription;
                player.PreviewSkill(i);
            }
        }
        if (!hovering)
        {
            foreach (ReadoutOnHover readoutObj in readoutObjects)
            {
                if (IsTouchingMouse(readoutObj.gameObject))
                {
                    readout.text = readoutObj.readoutText;
                    hovering = true;
                }
            }
        }
        if (!hovering) { readout.text = "..."; }

        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        for (int i = 0; i < skillButtons.Count; i++)
        {
            skillText[i].text = player.equippedSkills[i].skillName;
            energyCost[i].text = player.equippedSkills[i].cost.ToString();

            if (player.equippedSkills[i].cost > player.energy) 
            { skillButtons[i].GetComponentInChildren<Button>().interactable = false; energyIcon[i].SetActive(false); } 
            else 
            { skillButtons[i].GetComponentInChildren<Button>().interactable = true; energyIcon[i].SetActive(true); }
        }

        energyText.text = player.energy.ToString();
    }

    public bool IsTouchingMouse(GameObject g)
    {
        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return g.GetComponent<Collider2D>().OverlapPoint(point);
    }
}
