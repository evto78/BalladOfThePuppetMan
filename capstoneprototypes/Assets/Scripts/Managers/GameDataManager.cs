using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDataManager : MonoBehaviour
{
    [Header("Functional")]
    public List<PlayerSkill> startingSkills;
    public enum State { StartGame, StartCombat, TurnBasedCombat, RealTimeCombat, CombatEnd, RewardSelection, RoomSelection, GameOver}
    public State gameState = State.StartGame;
    public int areaID; // The ID number of the current area, since we only have suburbia rn, this should only ever be 0.

    [Header("References")]
    public PlayerActionManager player;
    public TimelineManager timeline;
    public AudioManager audioMan;
    public EnemySpawnManager enemySpawnMan;
    public RoomSelection roomSelection;
    public RewardManager rewardManager;

    private void Start()
    {
        ChangeState(State.StartGame);
    }

    public void ChangeState(State newState)
    {
        gameState = newState;
        switch (gameState)
        {
            case State.StartGame: 
                player.equippedSkills = startingSkills; //Equip the player
                audioMan.PlayMusic(0, 1, 1); // Start playing the ambience
                ChangeState(State.RoomSelection); // Open up room selection
                break;
            case State.StartCombat:
                timeline.StartCombat();
                break;
            case State.TurnBasedCombat: 
                break;
            case State.RealTimeCombat: 
                break;
            case State.CombatEnd: 
                ChangeState(State.RewardSelection);
                break;
            case State.RewardSelection: 
                ChangeState(State.RoomSelection); // !!! - FOR NOW! REWARD SELECTION IS SKIPPED TO GO STRAIGHT TO ROOM SELECTION! WHEN IMPLEMENTING REWARDS CHANGE THIS! - !!!
                break;
            case State.RoomSelection: 
                break;
            case State.GameOver:
                player.gameOverAnim.PlayAnim();
                break;
        }
    }

    private void Update()
    {
        UIUpdate();
    }

    void UIUpdate()
    {
        roomSelection.gameObject.SetActive(gameState == State.RoomSelection);
        rewardManager.gameObject.SetActive(gameState == State.RewardSelection);
    }

    public void QuitToDesktop()
    {
        Application.Quit();
    }
    public void QuitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
