using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    #region Header GAMEOBJECT REFERENCES
    [Space(10)]
    [Header("GAMEOBJECT REFERENCES")]
    #endregion
    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private TextMeshProUGUI messageTextTMP;
    [SerializeField] private CanvasGroup canvasGroup;

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
    private long gameScore;
    private int scoreMultiplier;
    private InstantiatedRoom bossRoom;
    private bool isFading = false;

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

        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;

        StaticEventHandler.OnPointsScored += StaticEventHandler_OnPointsScored;

        StaticEventHandler.OnMultiplierEvent += StaticEventHandler_OnMultiplierEvent;

        player.destroyedEvent.OnDestroyed += Player_OnDestroyed;

    }

    private void OnDisable() {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;

        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;

        StaticEventHandler.OnPointsScored -= StaticEventHandler_OnPointsScored;

        StaticEventHandler.OnMultiplierEvent -= StaticEventHandler_OnMultiplierEvent;

        player.destroyedEvent.OnDestroyed -= Player_OnDestroyed;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs){
        SetCurrentRoom(roomChangedEventArgs.room);
    }

    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs){
        RoomEnemiesDefeated();
    }

    private void StaticEventHandler_OnPointsScored(PointsScoredAgrs pointsScoredAgrs){
        gameScore += pointsScoredAgrs.points * scoreMultiplier;

        StaticEventHandler.CallScoreChangedEvent(gameScore, scoreMultiplier);
    }

    private void StaticEventHandler_OnMultiplierEvent(MultiplierArgs multiplierArgs){
        if (multiplierArgs.multiplier){
            scoreMultiplier++;
        } else {
            scoreMultiplier--;
        }
        scoreMultiplier = Mathf.Clamp(scoreMultiplier, 1, 30);

        StaticEventHandler.CallScoreChangedEvent(gameScore, scoreMultiplier);
    }

    

    private void Player_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs){
        previousGameState = gameState;
        gameState = GameState.gameLost;
    }

    private void Start(){
        previousGameState = GameState.gameStarted;
        gameState = GameState.gameStarted;

        gameScore = 0;

        scoreMultiplier = 1;

        StartCoroutine(Fade(0f, 1f, 0f, Color.black));
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
                RoomEnemiesDefeated();
                break;
            case GameState.playingLevel:
                if (Input.GetKeyDown(KeyCode.Escape)){
                    PauseGameMenu();
                }
                if (Input.GetKeyDown(KeyCode.Tab)){
                    DisplayDungeonOverviewMap();
                }
                break;
            case GameState.engagingEnemies:
                if (Input.GetKeyDown(KeyCode.Escape)){
                    PauseGameMenu();
                }
                break;

            case GameState.dungeonOverviewMap:
                if (Input.GetKeyUp(KeyCode.Tab)){
                    DungeonMap.Instance.ClearDungeonOverViewMap();
                }
                break;

            case GameState.bossStage:
                if (Input.GetKeyDown(KeyCode.Escape)){
                    PauseGameMenu();
                }
                if (Input.GetKeyDown(KeyCode.Tab)){
                    DisplayDungeonOverviewMap();
                }
                break;

            case GameState.engagingBoss:
                if (Input.GetKeyDown(KeyCode.Escape)){
                    PauseGameMenu();
                }
                break;

            case GameState.levelCompleted:
                StartCoroutine(LevelCompleted());
                break;

            case GameState.gameWon:
                if (previousGameState != GameState.gameWon){
                    StartCoroutine(GameWon());
                }
                break;

            case GameState.gameLost:
                if (previousGameState != GameState.gameLost){
                    StartCoroutine(GameLost());
                }
                break;
            case GameState.restartGame:
                RestartGame();
                break;

            case GameState.gamePaused:
                if (Input.GetKeyDown(KeyCode.Escape)){
                    PauseGameMenu();
                }
                break;
        }
    }

    public void SetCurrentRoom(Room room){
        previousRoom = currentRoom;
        currentRoom = room;

    }

    private void RoomEnemiesDefeated(){
        bool isDungeonClearOfRegularEnemies = true;
        bossRoom = null;

        foreach (KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary){
            if (keyValuePair.Value.roomNodeType.isBossRoom){
                bossRoom = keyValuePair.Value.instantiatedRoom;
                continue;
            }
            if (!keyValuePair.Value.isClearedOfEnemies){
                isDungeonClearOfRegularEnemies = false;
                break;
            }
        }

        if ((isDungeonClearOfRegularEnemies && bossRoom == null) || (isDungeonClearOfRegularEnemies && bossRoom.room.isClearedOfEnemies)){
            if (currentDungeonLevelListIndex < dungeonLevelList.Count -1){
                gameState = GameState.levelCompleted;
            } else {
                gameState = GameState.gameWon;
            }
        }

        else if (isDungeonClearOfRegularEnemies){
            gameState = GameState.bossStage;

            StartCoroutine(BossStage());
        }
    }

    public void PauseGameMenu(){
        if (gameState != GameState.gamePaused){
            pauseMenu.SetActive(true);
            GetPlayer().playerControl.DisablePlayer();

            previousGameState = gameState;
            gameState = GameState.gamePaused;
        }
        else if (gameState == GameState.gamePaused){
            pauseMenu.SetActive(false);
            GetPlayer().playerControl.EnablePlayer();

            gameState = previousGameState;
            previousGameState = GameState.gamePaused;
        }
    }

    private void DisplayDungeonOverviewMap(){
        if (isFading) return;

        DungeonMap.Instance.DisplayDungeonOverViewMap();
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

        StartCoroutine(DisplayDungeonLevelText());
    }

    private IEnumerator DisplayDungeonLevelText(){
        StartCoroutine(Fade(0f, 1f, 0f, Color.black));

        GetPlayer().playerControl.DisablePlayer();

        string messageText = "LEVEL " + (currentDungeonLevelListIndex + 1).ToString() + "\n\n" + dungeonLevelList
            [currentDungeonLevelListIndex].levelName.ToUpper();

        yield return StartCoroutine(DisplayMessageRoutine(messageText, Color.white, 2f));

        GetPlayer().playerControl.EnablePlayer();

        yield return StartCoroutine(Fade(1f, 0f, 2f, Color.black));

    }

    private IEnumerator DisplayMessageRoutine(string text, Color textColor, float displaySeconds){
        messageTextTMP.SetText(text);
        messageTextTMP.color = textColor;

        if (displaySeconds > 0f){
            float timer = displaySeconds;

            while (timer > 0f && !Input.GetKeyDown(KeyCode.Return)){
                timer -= Time.deltaTime;
                yield return null;
            }
        } else {
            while (!Input.GetKeyDown(KeyCode.Return)){
                yield return null;
            }
        }

        messageTextTMP.SetText("");
    }

    private IEnumerator BossStage(){
        bossRoom.gameObject.SetActive(true);
        bossRoom.UnlockDoors(0f);

        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        string bossStageText = "WELL DONE " + GameResources.Instance.currentPlayerSO.playerName + "! YOU'VE SURVIVED \n NOW  FIND AND DEFEAT THE BOSS GOOD LUCK!";
        yield return StartCoroutine(DisplayMessageRoutine(bossStageText, Color.white, 5f));

        yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));
    }

    private IEnumerator LevelCompleted(){
        gameState = GameState.playingLevel;

        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        string levelCompletedText = "WELL DONE " + GameResources.Instance.currentPlayerSO.playerName + "\n YOU'VE SURVIVED THIS DUNGEON LEVEL";
        string nextLevelText = "PRESS ENTER TO DESCEND FURTHER INTO THE DUNGEON";
        yield return StartCoroutine(DisplayMessageRoutine(levelCompletedText, Color.white, 5f));
        yield return StartCoroutine(DisplayMessageRoutine(nextLevelText, Color.white, 5f));

        yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        while (!Input.GetKeyDown(KeyCode.Return)){
            yield return null;
        }

        yield return null;

        currentDungeonLevelListIndex++;

        PlayDungeonLevel(currentDungeonLevelListIndex);
    }

    public IEnumerator Fade(float startFadeAlpha, float targetFadeAlpha, float fadeSeconds, Color backgroundColor){

        isFading = true;
        Image image = canvasGroup.GetComponent<Image>();
        image.color = backgroundColor;

        float time = 0;

        while (time <= fadeSeconds){
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startFadeAlpha, targetFadeAlpha, time / fadeSeconds);
            yield return null;
        }
        
        isFading = false;

    }

    private IEnumerator GameWon(){
        previousGameState = GameState.gameWon;

        GetPlayer().playerControl.DisablePlayer();

        int rank = HighScoreManager.Instance.GetRank(gameScore);

        string rankText;

        if (rank > 0 && rank <= Settings.numberOfHighScoresToSave){
            rankText = "YOUR SCORE IS RANKED " + rank.ToString("#0") + " IIN THE TOP " + Settings.numberOfHighScoresToSave.ToString("#0");

            string name = GameResources.Instance.currentPlayerSO.playerName;

            if (name == ""){
                name = playerDetails.playerCharacterName.ToUpper();
            }

            HighScoreManager.Instance.AddScore(new Score(){
                playerName = name,
                levelDescription = "LEVEL " + (currentDungeonLevelListIndex + 1).ToString() + "-" + GetCurrentDungeonLevel().levelName.ToUpper(),
                playerScore = gameScore
            }, rank);
        } else {
            rankText = "YOUR SCORE ISN'T RANKED IN THE TOP " + Settings.numberOfHighScoresToSave.ToString("#0");
        }

        yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));

        yield return StartCoroutine(DisplayMessageRoutine("WELL DONE " + GameResources.Instance.currentPlayerSO.playerName + "! YOU HAVE DEFEATED THE DUNGEON", Color.white, 3f));

        yield return StartCoroutine(DisplayMessageRoutine("YOU SCORED " + gameScore.ToString("###,###0") + "\n\n" + rankText, Color.white, 4f));

        yield return StartCoroutine(DisplayMessageRoutine("PRESS RETURN TO RESTART THE GAME", Color.white, 0f));

        gameState = GameState.restartGame;
    }

    private IEnumerator GameLost(){
        previousGameState = GameState.gameLost;

        GetPlayer().playerControl.DisablePlayer();

        int rank = HighScoreManager.Instance.GetRank(gameScore);

        string rankText;

        if (rank > 0 && rank <= Settings.numberOfHighScoresToSave){
            rankText = "YOUR SCORE IS RANKED " + rank.ToString("#0") + " IIN THE TOP " + Settings.numberOfHighScoresToSave.ToString("#0");

            string name = GameResources.Instance.currentPlayerSO.playerName;

            if (name == ""){
                name = playerDetails.playerCharacterName.ToUpper();
            }

            HighScoreManager.Instance.AddScore(new Score(){
                playerName = name,
                levelDescription = "LEVEL " + (currentDungeonLevelListIndex + 1).ToString() + "-" + GetCurrentDungeonLevel().levelName.ToUpper(),
                playerScore = gameScore
            }, rank);
        } else {
            rankText = "YOUR SCORE ISN'T RANKED IN THE TOP " + Settings.numberOfHighScoresToSave.ToString("#0");
        }

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));

        Enemy[] enemyArray = GameObject.FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemyArray){
            enemy.gameObject.SetActive(false);
        }

        string lostText = "NICE TRY " + GameResources.Instance.currentPlayerSO.playerName + "\n BUT YOU LOST!";
        yield return StartCoroutine(DisplayMessageRoutine(lostText, Color.white, 4f));

        yield return StartCoroutine(DisplayMessageRoutine("YOU SCORED " + gameScore.ToString("###,###0") + "\n\n" + rankText, Color.white, 4f));

        yield return StartCoroutine(DisplayMessageRoutine("PRESS ENTER TO RESTART GAME", Color.white, 0f));

        gameState = GameState.restartGame;
    }

    private void RestartGame(){
        SceneManager.LoadScene("MainMenuScene");
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
        HelperUtilities.ValidateCheckNullValue(this, nameof(pauseMenu), pauseMenu);
        HelperUtilities.ValidateCheckNullValue(this, nameof(messageTextTMP), messageTextTMP);
        HelperUtilities.ValidateCheckNullValue(this, nameof(canvasGroup), canvasGroup);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
    #endregion
}
