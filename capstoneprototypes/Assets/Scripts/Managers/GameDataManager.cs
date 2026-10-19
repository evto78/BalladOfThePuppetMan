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

    [Header("References")]
    public PlayerActionManager player;
    public TimelineManager timeline;
    public AudioManager audioMan;
    public EnemySpawnManager enemySpawnMan;

    private void Start()
    {
        ChangeState(State.StartGame);
    }

    public void ChangeState(State newState)
    {
        switch (newState)
        {
            case State.StartGame: 
                player.equippedSkills = startingSkills;
                break;
            case State.StartCombat: 
                break;
            case State.TurnBasedCombat: 
                break;
            case State.RealTimeCombat: 
                break;
            case State.CombatEnd: 
                break;
            case State.RewardSelection: 
                break;
            case State.RoomSelection: 
                break;
            case State.GameOver: 
                break;
        }
    }

    private void Update()
    {
        UIUpdate();
    }

    void UIUpdate()
    {

    }
}
