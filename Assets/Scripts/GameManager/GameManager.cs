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
    private Room currentRoom;
    private Room previousRoom;
    private PlayerDetailsSO playerDetails;
    private Player player;
    [HideInInspector] public GameState gameState;
    [HideInInspector] public GameState previousGameState;

    protected override void Awake(){
        base.Awake();

        playerDetails = GameResources.Instance.currentPlayerSO.playerDetails;

        InstantiatePlayer();
    }

    private void InstantiatePlayer(){
        GameObject playerGameObject = Instantiate(playerDetails.playerPrefab);

        player = playerGameObject.GetComponent<Player>();
        
        player.Initialize(playerDetails);
    }

    private void OnEnable() {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable() {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs){
        SetCurrentRoom(roomChangedEventArgs.room);
    }

    private void Start(){
        previousGameState = GameState.gameStarted;
        gameState = GameState.gameStarted;
    }

    private void Update(){
        HandleGameState();

        // if (Input.GetKeyDown(KeyCode.P)){
        //     gameState = GameState.gameStarted;
        // }
    }

    private void HandleGameState(){
        switch(gameState){
            case GameState.gameStarted:
                PlayDungeonLevel(currentDungeonLevelListIndex);
                gameState = GameState.playingLevel;
                break;
        }
    }

    public void SetCurrentRoom(Room room){
        previousRoom = currentRoom;
        currentRoom = room;

    }

    private void PlayDungeonLevel(int dungeonLeveListIndex){

        bool dungeonBuiltSuccessful = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLeveListIndex]);

        if(!dungeonBuiltSuccessful){
            Debug.LogError("Couldn't build dungeon from specified rooms and node graphs");
        }

        StaticEventHandler.CallRoomChangedEvent(currentRoom);

        Vector3 Playerposition = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f,
            (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f, 0f);

        player.gameObject.transform.position = HelperUtilities.GetSpawnPositionNearestToPlayer(Playerposition);
    }

    public Room GetCurrentRoom(){
        return currentRoom;
    }

    public Player GetPlayer(){
        return player;
    }

    public Sprite GetPlayerMiniMapIcon(){
        return playerDetails.playerMiniMapIcon;
    }

    public DungeonLevelSO GetCurrentDungeonLevel(){
        return dungeonLevelList[currentDungeonLevelListIndex];
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate(){
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
    #endregion
}
