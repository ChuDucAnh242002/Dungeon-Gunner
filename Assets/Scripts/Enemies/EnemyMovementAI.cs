using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyMovementAI : MonoBehaviour
{
    [SerializeField] private MovementDetailsSO movementDetails;
    private Enemy enemy;
    private Stack<Vector3> movementSteps = new Stack<Vector3>();
    private Vector3 playerReferencePosition;
    private Coroutine moveEnemyRoutine;
    private float currentEnemyPathRebuildCooldown;
    private WaitForFixedUpdate waitForFixedUpdate;
    [HideInInspector] public float moveSpeed;
    private bool chasePlayer = false;
    [HideInInspector] public int updateFrameNumber = 1;

    private void Awake(){
        enemy = GetComponent<Enemy>();

        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Start(){
        waitForFixedUpdate = new WaitForFixedUpdate();
        playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
    }

    private void Update(){
        MoveEnemy();
    }

    private void MoveEnemy(){
        float chaseDistance = enemy.enemyDetails.chaseDistance;
        Vector3 playerPosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

        currentEnemyPathRebuildCooldown -= Time.deltaTime;

        if (!chasePlayer && Vector3.Distance(transform.position, playerPosition) < chaseDistance){
            chasePlayer = true;
        }

        if (!chasePlayer) return;

        if (Time.frameCount % Settings.targetFrameRateToSpreadPathfindingOver != updateFrameNumber) return;

        if (currentEnemyPathRebuildCooldown <= 0f || 
            (Vector3.Distance(playerReferencePosition, playerPosition) > Settings.playerMoveDistanceToRebuildPath)){
            currentEnemyPathRebuildCooldown = Settings.enemyPathRebulidCooldown;

            playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

            CreatePath();

            if (movementSteps != null){
                if (moveEnemyRoutine != null){
                    enemy.idleEvent.CallIdleEvent();
                    StopCoroutine(moveEnemyRoutine);
                }

                moveEnemyRoutine = StartCoroutine(MoveEnemyRoutine(movementSteps));
            }
        }
    }

    private IEnumerator MoveEnemyRoutine(Stack<Vector3> movementSteps){
        while(movementSteps.Count > 0){
            Vector3 nextPosition = movementSteps.Pop();

            while (Vector3.Distance(nextPosition, transform.position) > 0.2f){
                enemy.movementToPositionEvent.CallMovementToPositionEvent(nextPosition, 
                    transform.position, moveSpeed, 
                    (nextPosition - transform.position).normalized, false);
                yield return waitForFixedUpdate;
            }

            yield return waitForFixedUpdate;

        }

        enemy.idleEvent.CallIdleEvent();
    }

    private void CreatePath(){
        Room currentRoom = GameManager.Instance.GetCurrentRoom();
        Vector3 playerPosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

        Grid grid = currentRoom.instantiatedRoom.grid;

        Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);
        Vector3Int playerCellPosition = grid.WorldToCell(playerPosition);
        Vector3Int playerGridPosition = GetNearestNonObstaclePlayerPosition(currentRoom, playerPosition, playerCellPosition);

        movementSteps = AStar.BuildPath(currentRoom, enemyGridPosition, playerGridPosition);

        if (movementSteps != null){
            movementSteps.Pop();
        } else {
            enemy.idleEvent.CallIdleEvent();
        }
    }

    public void SetUpdateFrameNumber(int updateFrameNumber){
        this.updateFrameNumber = updateFrameNumber;
    }

    // There are half collision type are marked as Obstacle
    private Vector3Int GetNearestNonObstaclePlayerPosition(Room currentRoom, Vector3 playerPosition, Vector3Int playerCellPosition){

        Vector2Int adjustedPlayerCellPosition = new Vector2Int(playerCellPosition.x - currentRoom.templateLowerBounds.x,
            playerCellPosition.y - currentRoom.templateLowerBounds.y);
        int obstacle = currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x, adjustedPlayerCellPosition.y];

        if (obstacle == 0){
            for (int i = -1; i <= 1; i++){
                for (int j = -1; j <= 1; j++){
                    if (j == 0 && i == 0) continue;

                    try {
                        obstacle = currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x + 1, adjustedPlayerCellPosition.y + 1];
                        if (obstacle != 0){
                            return new Vector3Int(playerCellPosition.x + i, playerCellPosition.y + j, 0);
                        }
                    } 
                    catch {
                        continue;
                    }
                }
            }
        }

        return playerCellPosition;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }
#endif
    #endregion
}
