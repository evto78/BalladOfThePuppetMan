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
    public Button[] skillReplacement, rewards;
    public GameObject skipButton;
    public PlayerActionManager player;
    public TimelineManager timeline;

    PlayerSkill selectedSkill;
    GameDataManager gdm;
    bool buttonsEnabled = true;
    


    //public TextMeshProUGUI[] currentSkillText;
    //public TextMeshProUGUI[] currentSkillEnergy;

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
            }
            buttonsEnabled = false;
            Debug.Log("Buttons disabled");
        }
    }

    public void RandomizeRewards()
    {
        for (int y = 0; y < rewards.Length; y++)
        {
            rewards[y].interactable = true;
        }
        skipButton.SetActive(true);
        //UpdatePlayerSkillsUI();

        //When we have more skills, this will include logic to make sure duplicates cannot appear
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

    //public void UpdatePlayerSkillsUI()
    //{
    //    for (int i = 0; i < player.equippedSkills.Count; i++)
    //    {
    //        currentSkillText[i].text = player.equippedSkills[i].skillName;
    //        currentSkillEnergy[i].text = $"{player.equippedSkills[i].cost}";
    //    }
    //}

    public void SkipSelection()
    {
        for (int i = 0; i < skillReplacement.Length; i++)
        {
            skillReplacement[i].interactable = true;
        }
        buttonsEnabled = true;
        gdm.ChangeState(GameDataManager.State.RoomSelection);
    }
}