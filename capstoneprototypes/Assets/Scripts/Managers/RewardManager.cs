using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour
{
    public GameObject rewardUI, rewardSelectionUI;
    public PlayerSkill[] skills;
    public DataList dl;
    public TextMeshProUGUI[] rewardText;
    public Button[] skillReplacement;
    public PlayerActionManager player;

    PlayerSkill selectedSkill;


    //Used for testing purposes only
    public TextMeshProUGUI[] currentSkillText;

    private void Start()
    {
        RandomizeRewards();
    }

    public void RandomizeRewards()
    {
        //When we have more skills, this will include logic to make sure duplicates cannot appear
        for (int i = 0; i < skills.Length; i++)
        {
            int randomSkillIndex = Random.Range(0, dl.allPlayerSkills.Count);
            skills[i] = dl.allPlayerSkills[randomSkillIndex];
            rewardText[i].text = skills[i].skillName;
        }
        rewardSelectionUI.SetActive(true);
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
        player.equippedSkills[slotIndex] = selectedSkill;

        UpdatePlayerSkillsUI();

        //Just for testing
        for (int i = 0; i < skillReplacement.Length; i++)
        {
            skillReplacement[i].interactable = false;
        }

        rewardSelectionUI.SetActive(false);
    }

    public void UpdatePlayerSkillsUI()
    {
        for (int i = 0; i < player.equippedSkills.Count; i++)
        {
            currentSkillText[i].text = player.equippedSkills[i].skillName;
        }
    }
}