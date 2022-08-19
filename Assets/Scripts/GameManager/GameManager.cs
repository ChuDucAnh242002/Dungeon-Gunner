using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    #region Header DUNGEON LEVELS

    [Space(10)]
    [Header("DUNGEON LEVELS")]

    #endregion
    #region  Tooltip
    [Tooltip("Populate with the dungeon level sciptable objects")]
    #endregion Tooltip
    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;

    #region Tooltip
    [Tooltip("Populate with starting the dungeon level for testing, first level =0")]
    #endregion Tooltip
    [SerializeField] private int currentDungeonLevelListIndex;
    [HideInInspector] public GameState gameState;

    private void Start(){
        gameState = GameState.gameStarted;
    }

    private void Update(){
        HandleGameState();

        if (Input.GetKeyDown(KeyCode.R)){
            gameState = GameState.gameStarted;
        }
    }

    private void HandleGameState(){
        switch(gameState){
            case GameState.gameStarted:
                PlayDungeonLevel(currentDungeonLevelListIndex);
                gameState = GameState.playingLevel;
                break;
        }
    }

    private void PlayDungeonLevel(int dungeonLeveListIndex){
        bool dungeonBuiltSuccessful = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLeveListIndex]);

        if(!dungeonBuiltSuccessful){
            Debug.LogError("Couldn't build dungeon from specified rooms and node graphs");
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate(){
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
    #endregion
}
