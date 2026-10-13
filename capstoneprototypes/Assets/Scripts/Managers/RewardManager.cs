using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour
{
    public GameObject rewardUI;
    public PlayerSkill[] skills;
    public DataList dl;
    public TextMeshProUGUI[] rewardText;
    public Button[] skillReplacement;

    PlayerSkill selectedSkill;


    //Used for testing purposes only
    public PlayerSkill[] playerSkills;
    public GameObject combat, rewards;
    public TextMeshProUGUI[] currentSkillText;

    public void RandomizeRewards()
    {
        //When we have more skills, this will include logic to make sure duplicates cannot appear
        for (int i = 0; i < skills.Length; i++)
        {
            int randomSkillIndex = Random.Range(0, dl.allPlayerSkills.Count);
            skills[i] = dl.allPlayerSkills[randomSkillIndex];
            rewardText[i].text = skills[i].skillName;
        }
        ChangeGameState(false);
    }

    public void SelectReward(int slotIndex)
    {
        selectedSkill = skills[slotIndex];

        //Just for testing
        for (int i = 0; i < skillReplacement.Length; i++)
        {
            skillReplacement[i].interactable = true;
        }
    }

    public void ReplaceSkill(int slotIndex)
    {
        playerSkills[slotIndex] = selectedSkill;

        for (int i = 0; i < playerSkills.Length; i++)
        {
            currentSkillText[i].text = playerSkills[i].skillName;
        }

        //Just for testing
        for (int i = 0; i < skillReplacement.Length; i++)
        {
            skillReplacement[i].interactable = false;
        }

        ChangeGameState(true);
    }

    
    //Function just for testing
    public void ChangeGameState(bool combatEnabled)
    {
        combat.SetActive(combatEnabled);
        rewards.SetActive(!combatEnabled);
    }
}