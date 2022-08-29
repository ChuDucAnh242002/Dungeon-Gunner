using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour
{
    // public RoomTemplateSO roomTemplateSO;
    private List<SpawnableObjectByLevel<EnemyDetailsSO>> testLevelSpawnList;
    private RandomSpawnableObject<EnemyDetailsSO> randomEnemyHelperClass;
    private List<GameObject> instantiatedEnemyList = new List<GameObject>();

    private void OnEnable() {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }
    private void OnDisable() {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs){
        if (instantiatedEnemyList != null && instantiatedEnemyList.Count > 0){
            foreach (GameObject enemy in instantiatedEnemyList){
                Destroy(enemy);
            }
        }

        RoomTemplateSO roomTemplate = DungeonBuilder.Instance.GetRoomTemplate(roomChangedEventArgs.room.templateID);

        if (roomTemplate != null){
            testLevelSpawnList = roomTemplate.enemiesByLevelList;

            randomEnemyHelperClass = new RandomSpawnableObject<EnemyDetailsSO>(testLevelSpawnList);
        }
    }

    private void Update(){
        if (Input.GetKeyDown(KeyCode.T)){

            EnemyDetailsSO enemyDetails = randomEnemyHelperClass.GetItem();

            if (enemyDetails != null){
                Vector3 spawnPositionNearestToPlayer = HelperUtilities.GetSpawnPositionNearestToPlayer(HelperUtilities.GetMouseWorldPosition());
                GameObject instantiatedEnemy = Instantiate(enemyDetails.enemyPrefab, spawnPositionNearestToPlayer, Quaternion.identity);
                instantiatedEnemyList.Add(instantiatedEnemy);
            }
        }
    }

    
}
