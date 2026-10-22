using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectionManager : MonoBehaviour
{
    [Header("References")]
    public List<SkillButton> skillButtons;
    public TextMeshProUGUI readout;
    public TextMeshProUGUI energyText;
    PlayerActionManager player;
    TargetingArrow tarArrow;
    public List<Sprite> skillSprites;
    public List<Sprite> skillSpritesHover;
    public List<Sprite> skillElements;
    GameDataManager gdm;


    [Header("Functional")]
    public float hoverXOffset; // how much should the skill buttons shift when selected?
    List<float> hoverProgress; // individual timers for each skill button to track how much it has been hovered over.
    List<Vector3> initialPos; // all of the skill buttons initial pos
    public List<ReadoutOnHover> readoutObjects;
    int targetingSkill = -1;

    private void Awake()
    {
        gdm = GameObject.Find("GameDataManager").GetComponent<GameDataManager>();
        player = GameObject.Find("Player").GetComponent<PlayerActionManager>();
        tarArrow = GetComponentInChildren<TargetingArrow>();

        hoverProgress = new List<float>();
        initialPos = new List<Vector3>();
        foreach(SkillButton button in skillButtons)
        {
            hoverProgress.Add(0f);
            initialPos.Add(button.transform.localPosition);
        }
    }

    void Update()
    {
        // If it isn't currently time to be selecting skills, make sure that no skill is currently trying to find a target.
        if (gdm.gameState != GameDataManager.State.TurnBasedCombat) { targetingSkill = -1; }

        // Make sure that player doesn't target an enemy without the conditionals below.
        player.targetedEnemy = null;

        bool hovering = false;
        player.timeline.StopPreviewSkillBlock();
        for (int i = 0; i < skillButtons.Count; i++)
        {
            if (targetingSkill == -1)
            {
                if (IsTouchingMouse(skillButtons[i].gameObject))
                {
                    hovering = true;
                    hoverProgress[i] += Time.deltaTime * 12;
                    readout.text = player.equippedSkills[i].skillDescription;
                    player.PreviewSkill(i);
                }
                else
                {
                    hoverProgress[i] -= Time.deltaTime * 24;
                }
            }
            else
            {
                hoverProgress[i] -= Time.deltaTime * 24;
            }
            hoverProgress[i] = Mathf.Clamp(hoverProgress[i], 0f, 1f);
        }
        if (!hovering)
        {
            foreach (ReadoutOnHover readoutObj in readoutObjects)
            {
                if (IsTouchingMouse(readoutObj.gameObject))
                {
                    readout.text = readoutObj.readoutText;
                    hovering = true;

                    if (targetingSkill != -1 && readoutObj.transform.parent.TryGetComponent<Enemy>(out Enemy enemyScript) && player.timeline.activeEnemies.Contains(enemyScript))
                    {
                        player.targetedEnemy = enemyScript;
                        if (Input.GetMouseButtonDown(0))
                        {
                            player.AttemptUseSkill(targetingSkill);
                            StartTargetingSkill(-1);
                        }
                    }
                }
            }
        }
        if (!hovering) { readout.text = "..."; }

        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        tarArrow.gameObject.SetActive(targetingSkill > -1);

        for (int i = 0; i < skillButtons.Count; i++)
        {
            SkillButton btn = skillButtons[i];

            btn.skillText.text = player.equippedSkills[i].skillName;
            btn.energyCost.text = player.equippedSkills[i].cost.ToString();

            int element = player.equippedSkills[i].elementalType.GetHashCode();
            btn.elementIcon.sprite = skillElements[element];
            btn.img.sprite = skillSprites[element];

            if (player.equippedSkills[i].cost > player.energy) 
            { btn.GetComponentInChildren<Button>().interactable = false; btn.energyIcon.SetActive(false); } 
            else 
            { btn.GetComponentInChildren<Button>().interactable = true; btn.energyIcon.SetActive(true); }

            if (targetingSkill == i) 
            { 
                hoverProgress[i] = 1;
                tarArrow.transform.position = btn.elementIcon.transform.position;
            }

            btn.transform.localPosition = initialPos[i] + Vector3.right * (hoverXOffset * hoverProgress[i]);
        }

        energyText.text = player.energy.ToString();
    }

    public void StartTargetingSkill(int skillSlot)
    {
        targetingSkill = -1; player.targetedEnemy = null;

        if (skillSlot == -1) { return; }

        if (!player.isSkillUseable(skillSlot)) { return; }

        switch (player.equippedSkills[skillSlot].target)
        {
            case PlayerSkill.TargetType.Self: player.AttemptUseSkill(skillSlot); break;
            case PlayerSkill.TargetType.AllEnemies: player.AttemptUseSkill(skillSlot); break;
            case PlayerSkill.TargetType.SingleEnemy: targetingSkill = skillSlot; break;
        }
    }

    public bool IsTouchingMouse(GameObject g)
    {
        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return g.GetComponent<Collider2D>().OverlapPoint(point);
    }
}
