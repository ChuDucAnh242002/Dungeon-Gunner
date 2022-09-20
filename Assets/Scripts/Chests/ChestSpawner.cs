using System.Collections.Generic;
using UnityEngine;

public class ChestSpawner : MonoBehaviour
{
    [System.Serializable]

    private struct RangeByLevel{
        public DungeonLevelSO dungeonLevel;
        [Range(0, 100)] public int min;
        [Range(0, 100)] public int max; 
    }

    #region Header CHEST PREFAB
    [Space(10)]
    [Header("CHEST PREFAB")]
    #endregion
    [SerializeField] private GameObject chestPrefab;

    #region Header CHEST SPAWN CHANCE
    [Space(10)]
    [Header("CHEST SPAWN CHANCE")]
    #endregion
    [SerializeField] [Range(0, 100)] private int chestSpawnChanceMin;
    [SerializeField] [Range(0, 100)] private int chestSpawnChanceMax;
    [SerializeField] private List<RangeByLevel> chestSpawnChanceByLevelList;

    #region Header CHEST SPAWN DETAILS
    [Space(10)]
    [Header("CHEST SPAWN DETAILS")]
    #endregion
    [SerializeField] private ChestSpawnEvent chestSpawnEvent;
    [SerializeField] private ChestSpawnPosition chestSpawnPosition;
    [SerializeField] [Range(0, 3)] private int numberOfItemsToSpawnMin;
    [SerializeField] [Range(0, 3)] private int numberOfItemsToSpawnMax;

    #region Header CHEST CONTENT DETAILS
    [Space(10)]
    [Header("CHEST CONTENT DETAILS")]
    #endregion
    [SerializeField] private List<SpawnableObjectByLevel<WeaponDetailsSO>> weaponSpawnByLevelList;
    [SerializeField] private List<RangeByLevel> healthSpawnByLevelList;
    [SerializeField] private List<RangeByLevel> ammoSpawnByLevelList;

    private bool chestSpawned = false;
    private Room chestRoom;

    private void OnEnable(){
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;

        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;
    }
    private void OnDisable(){
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;

        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs){
        if (chestRoom == null){
            chestRoom = GetComponentInParent<InstantiatedRoom>().room;
        }

        if (!chestSpawned && chestSpawnEvent == ChestSpawnEvent.onRoomEntry && chestRoom == roomChangedEventArgs.room){
            SpawnChest();
        }
    }

    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs){
        if (chestRoom == null){
            chestRoom = GetComponentInParent<InstantiatedRoom>().room;
        }
        if (!chestSpawned && chestSpawnEvent == ChestSpawnEvent.onEnemiesDefeated && chestRoom == roomEnemiesDefeatedArgs.room){
            SpawnChest();
        }
    }

    private void SpawnChest(){
        chestSpawned = true;

        if (!RandomSpawnChest()) return;

        GetItemsToSpawn(out int ammoNum, out int healthNum, out int weaponNum);

        GameObject chestGameObject = Instantiate(chestPrefab, this.transform);

        if (chestSpawnPosition == ChestSpawnPosition.atSpawnerPosition){
            chestGameObject.transform.position = this.transform.position;
        }
        else if (chestSpawnPosition == ChestSpawnPosition.atPlayerPosition)
        {
            Vector3 spawnPosition = HelperUtilities.GetSpawnPositionNearestToPlayer(GameManager.Instance.GetPlayer().transform.position);

            Vector3 variation = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);

            chestGameObject.transform.position = spawnPosition + variation;
        }

        Chest chest = chestGameObject.GetComponent<Chest>();

        if (chestSpawnEvent == ChestSpawnEvent.onRoomEntry){
            chest.Initialize(false, GetHealthPercentToSpawn(healthNum), GetWeaponDetailsToSpawn(weaponNum), GetAmmoPercentToSpawn(ammoNum));
        } else {
            chest.Initialize(true, GetHealthPercentToSpawn(healthNum), GetWeaponDetailsToSpawn(weaponNum), GetAmmoPercentToSpawn(ammoNum));
        }
    }

    private bool RandomSpawnChest(){
        int chancePercent = Random.Range(chestSpawnChanceMin, chestSpawnChanceMax + 1);

        foreach (RangeByLevel rangeByLevel in chestSpawnChanceByLevelList){
            if (rangeByLevel.dungeonLevel == GameManager.Instance.GetCurrentDungeonLevel()){
                chancePercent = Random.Range(rangeByLevel.min, rangeByLevel.max + 1);
                break;
            }
        }

        int randomPercent = Random.Range(1, 100 + 1);

        if (randomPercent <= chancePercent){
            return true;
        } else {
            return false;
        }
    }

    private void GetItemsToSpawn(out int ammo, out int health, out int weapons){
        ammo = 0;
        health = 0;
        weapons = 0;

        int numberOfItemsToSpawn = Random.Range(numberOfItemsToSpawnMin, numberOfItemsToSpawnMax + 1);

        int choice;

        if (numberOfItemsToSpawn == 1){
            // choice = Random.Range(0, 3);
            // if (choice == 0){
            weapons++;
            return;
            // }
            // if (choice == 1){
            //     ammo++;
            //     return;
            // }
            // if (choice == 2){
            //     health++;
            //     return;
            // }
        }
        else if (numberOfItemsToSpawn == 2){
            choice = Random.Range(0, 3);
            // if (choice == 0){
            //     weapons++;
            //     ammo++;
            //     return;
            // }
            // if (choice == 1){
            if (choice == 0){
                ammo++;
            }
            else if (choice == 1){
                health++;
            }
            return;
            // }
            // if (choice == 2){
            //     health++;
            //     weapons++;
            //     return;
            // }
        }
        else if (numberOfItemsToSpawn >= 3){
            weapons++;
            ammo++;
            health++;
            return;
        }
        
    }

    private int GetHealthPercentToSpawn(int healthNum){
        if (healthNum == 0) return 0;

        foreach (RangeByLevel spawnPercentByLevel in healthSpawnByLevelList){
            if (spawnPercentByLevel.dungeonLevel == GameManager.Instance.GetCurrentDungeonLevel()){
                return Random.Range(spawnPercentByLevel.min, spawnPercentByLevel.max);
            }
        }

        return 0;
    }

    private WeaponDetailsSO GetWeaponDetailsToSpawn(int weaponNum){
        if (weaponNum == 0) return null;

        RandomSpawnableObject<WeaponDetailsSO> weaponRandom = new RandomSpawnableObject<WeaponDetailsSO>(weaponSpawnByLevelList);

        WeaponDetailsSO weaponDetails = weaponRandom.GetItem();

        return weaponDetails;
    }

    private int GetAmmoPercentToSpawn(int ammoNum){
        if (ammoNum == 0) return 0;

        foreach (RangeByLevel spawnPercentByLevel in ammoSpawnByLevelList){
            if (spawnPercentByLevel.dungeonLevel == GameManager.Instance.GetCurrentDungeonLevel()){
                return Random.Range(spawnPercentByLevel.min, spawnPercentByLevel.max);
            }
        }

        return 0;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate(){
    
        HelperUtilities.ValidateCheckNullValue(this, nameof(chestPrefab), chestPrefab);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(chestSpawnChanceMin), chestSpawnChanceMin, nameof(chestSpawnChanceMax), chestSpawnChanceMax, true);

        if (chestSpawnChanceByLevelList != null && chestSpawnChanceByLevelList.Count > 0){
            
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(chestSpawnChanceByLevelList), chestSpawnChanceByLevelList);

            foreach (RangeByLevel rangeByLevel in chestSpawnChanceByLevelList){
                HelperUtilities.ValidateCheckNullValue(this, nameof(rangeByLevel.dungeonLevel), rangeByLevel.dungeonLevel);
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(rangeByLevel.min), rangeByLevel.min, nameof(rangeByLevel.max), rangeByLevel.max, true);

            }
        }
        
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(numberOfItemsToSpawnMin), numberOfItemsToSpawnMax, nameof(numberOfItemsToSpawnMax), numberOfItemsToSpawnMax, true);

        if (weaponSpawnByLevelList != null && weaponSpawnByLevelList.Count > 0){
            foreach (SpawnableObjectByLevel<WeaponDetailsSO> weaponDetailsByLevel in weaponSpawnByLevelList){
                HelperUtilities.ValidateCheckNullValue(this, nameof(weaponDetailsByLevel.dungeonLevel), weaponDetailsByLevel.dungeonLevel);

                foreach (SpawnableObjectRatio<WeaponDetailsSO> weaponRatio in weaponDetailsByLevel.spawnableObjectRatioList){
                    HelperUtilities.ValidateCheckNullValue(this, nameof(weaponRatio.dungeonObject), weaponRatio.dungeonObject);
                    HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponRatio.ratio), weaponRatio.ratio, true);
                }
            }
        }

        if (healthSpawnByLevelList != null && healthSpawnByLevelList.Count > 0){
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(healthSpawnByLevelList), healthSpawnByLevelList);

            foreach (RangeByLevel rangeByLevel in healthSpawnByLevelList){
                HelperUtilities.ValidateCheckNullValue(this, nameof(rangeByLevel.dungeonLevel), rangeByLevel.dungeonLevel);
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(rangeByLevel.min), rangeByLevel.min, nameof(rangeByLevel.max), rangeByLevel.max, true);
            }
        }

        if (ammoSpawnByLevelList != null && ammoSpawnByLevelList.Count > 0){
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(ammoSpawnByLevelList), ammoSpawnByLevelList);
            foreach (RangeByLevel rangeByLevel in ammoSpawnByLevelList){
                HelperUtilities.ValidateCheckNullValue(this, nameof(RangeByLevel.dungeonLevel), rangeByLevel.dungeonLevel);
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(rangeByLevel.min), rangeByLevel.min, nameof(rangeByLevel.max), rangeByLevel.max, true);
            }
        }
    }
#endif
    #endregion
}
