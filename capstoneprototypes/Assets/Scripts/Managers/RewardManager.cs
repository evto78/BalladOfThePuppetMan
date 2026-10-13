using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public GameObject rewardUI;
    public PlayerSkill[] skills;
    public GameObject skillReplacementUI;
    public DataList dl;

    PlayerSkill selectedSkill;


    //Used for testing purposes only
    PlayerSkill[] playerSkills;
    public GameObject combat, rewards;

    //Used for testing purposes only
    private void Start()
    {
        playerSkills = new PlayerSkill[4];

        for (int i = 0; i < playerSkills.Length; i++)
        {
            playerSkills[i] = dl.allPlayerSkills[i];
        }
    }

    
    public void RandomizeRewards()
    {
        for (int i = 0; i < skills.Length; i++)
        {
            int randomSkillIndex = Random.Range(0, dl.allPlayerSkills.Count);
            skills[i] = dl.allPlayerSkills[randomSkillIndex];
            ChangeGameState(false);
        }
    }

    public void SelectReward(int slotIndex)
    {
        selectedSkill = skills[slotIndex];
    }

    public void ReplaceSkill(int slotIndex)
    {
        playerSkills[slotIndex] = selectedSkill;
        ChangeGameState(true);
    }

    
    //Function just for testing
    public void ChangeGameState(bool combatEnabled)
    {
        combat.SetActive(combatEnabled);
        rewards.SetActive(!combatEnabled);
    }
}