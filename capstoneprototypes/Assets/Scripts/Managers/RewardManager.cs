using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour
{
    public PlayerSkill[] skills;
    public DataList dl;
    public TextMeshProUGUI[] rewardText;
    public TextMeshProUGUI[] rewardEnergy;
    public Image[] rewardElement;
    public Button[] skillReplacement, rewards;
    public GameObject skipButton;
    public PlayerActionManager player;
    public TimelineManager timeline;
    public GameObject timelineObject, liveTimelineObject;

    PlayerSkill selectedSkill;
    GameDataManager gdm;
    bool buttonsEnabled = true;

    public Sprite[] buttonSprites, elementSprites;
    

    private void Awake()
    {
        gdm = GameObject.Find("GameDataManager").GetComponent<GameDataManager>();
    }

    private void Update()
    {
        if (gdm.gameState == GameDataManager.State.RewardSelection && buttonsEnabled)
        {
            for (int i = 0; i < skillReplacement.Length; i++)
            {
                skillReplacement[i].interactable = false;
                timelineObject.SetActive(false);
                liveTimelineObject.SetActive(false);
            }
            buttonsEnabled = false;
        }
    }

    public void RandomizeRewards()
    {
        for (int y = 0; y < rewards.Length; y++)
        {
            rewards[y].interactable = true;
        }
        skipButton.SetActive(true);

        for (int i = 0; i < skills.Length; i++)
        {
            bool duplicate = true;

            while (duplicate) //Stops duplicate skills from appearing
            {
                int randomSkillIndex = Random.Range(0, dl.allPlayerSkills.Count);
                skills[i] = dl.allPlayerSkills[randomSkillIndex];

                int differenceCount = 0;

                for (int j = 0; j < player.equippedSkills.Count; j++)
                {
                    if (skills[i] != player.equippedSkills[j])
                    {
                        differenceCount++;
                    }
                }

                switch (i)
                {
                    case 0:
                        if (differenceCount == 4)
                        {
                            duplicate = false;
                        }
                        break;
                    case 1:
                        if (skills[i] != skills[0] && differenceCount == 4)
                        {
                            duplicate = false;
                        }
                        break;
                    case 2:
                        if (skills[i] != skills[0] && skills[i] != skills[1] && differenceCount == 4)
                        {
                            duplicate = false;
                        }
                        break;
                }
            }
            
            rewardText[i].text = skills[i].skillName;
            rewardEnergy[i].text = skills[i].cost.ToString();
            switch (skills[i].elementalType)
            {
                case ElementalTypes.Power:
                    rewardElement[i].sprite = elementSprites[0];
                    rewards[i].image.sprite = buttonSprites[0];
                    break;
                case ElementalTypes.Money:
                    rewardElement[i].sprite = elementSprites[1];
                    rewards[i].image.sprite = buttonSprites[1];
                    break;
                case ElementalTypes.Fame:
                    rewardElement[i].sprite = elementSprites[2];
                    rewards[i].image.sprite = buttonSprites[2];
                    break;
                case ElementalTypes.Corruption:
                    rewardElement[i].sprite = elementSprites[3];
                    rewards[i].image.sprite = buttonSprites[3];
                    break;
                case ElementalTypes.Purity:
                    rewardElement[i].sprite = elementSprites[4];
                    rewards[i].image.sprite = buttonSprites[4];
                    break;
            }
        }
    }

    public void SelectReward(int slotIndex)
    {
        selectedSkill = skills[slotIndex];

        for (int i = 0; i < skillReplacement.Length; i++)
        {
            skillReplacement[i].interactable = true;
        }

        for (int j = 0; j < rewards.Length; j++)
        {
            if(j != slotIndex)
            {
                rewards[j].interactable = false;
            }
        }
        skipButton.SetActive(false);

    }

    public void ReplaceSkill(int slotIndex)
    {
        if (!timeline.combatActive) {
            player.equippedSkills[slotIndex] = selectedSkill;

            //UpdatePlayerSkillsUI();

            for (int i = 0; i < skillReplacement.Length; i++)
            {
                skillReplacement[i].interactable = false;
            }

            SkipSelection();
        }
    }

    public void SkipSelection()
    {
        for (int i = 0; i < skillReplacement.Length; i++)
        {
            skillReplacement[i].interactable = true;
        }
        buttonsEnabled = true;
        timelineObject.SetActive(true);
        gdm.ChangeState(GameDataManager.State.RoomSelection);
    }
}