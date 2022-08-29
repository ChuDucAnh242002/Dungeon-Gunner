using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour
{
    public RoomTemplateSO roomTemplateSO;
    private List<SpawnableObjectByLevel<EnemyDetailsSO>> testLevelSpawnList;
    private RandomSpawnableObject<EnemyDetailsSO> randomEnemyHelperClass;
    private GameObject instantiatedEnemy;

    private void Start(){
        testLevelSpawnList = roomTemplateSO.enemiesByLevelList;

        randomEnemyHelperClass = new RandomSpawnableObject<EnemyDetailsSO>(testLevelSpawnList);
    }

    private void Update(){
        if (Input.GetKeyDown(KeyCode.T)){
            if (instantiatedEnemy != null){
                Destroy(instantiatedEnemy);
            }

            EnemyDetailsSO enemyDetails = randomEnemyHelperClass.GetItem();

            if (enemyDetails != null){
                Vector3 spawnPositionNearestToPlayer = HelperUtilities.GetSpawnPositionNearestToPlayer(HelperUtilities.GetMouseWorldPosition());
                instantiatedEnemy = Instantiate(enemyDetails.enemyPrefab, spawnPositionNearestToPlayer, Quaternion.identity);
            }
        }
    }
}
